using Arribbaa.MimicParty.InteropBootstrap;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Reflection;
using System.Runtime.Loader;

int checks = 0;
void Check(bool value, string text) { if (!value) throw new Exception("FAIL: " + text); Console.WriteLine("PASS: " + text); checks++; }
void Refuses(Action action, string text) { bool rejected = false; try { action(); } catch { rejected = true; } Check(rejected, text); }
bool Loads(byte[] data, string? type = null) {
    var context = new AssemblyLoadContext(Guid.NewGuid().ToString(), true);
    try { var a = context.LoadFromStream(new MemoryStream(data)); if (type != null) a.GetType(type, true); return true; }
    catch (BadImageFormatException) { return false; }
    finally { context.Unload(); }
}
byte[] Fixture(bool field = false, bool collision = false, string name = "UnityEngine.CoreModule", bool unexpected = false) {
    using var module = new ModuleDefUser(name + ".dll");
    var assembly = new AssemblyDefUser(name, new Version(0, 0, 0, 0)); assembly.Modules.Add(module);
    foreach (string n in new[] { "<>O", "<>O", "<>O", "<>c", "<>c" }) {
        var t = new TypeDefUser("", n, module.CorLibTypes.Object.TypeDefOrRef) { Attributes = dnlib.DotNet.TypeAttributes.NotPublic | dnlib.DotNet.TypeAttributes.Abstract | dnlib.DotNet.TypeAttributes.Sealed };
        module.Types.Add(t);
    }
    if (field) module.Types[2].Fields.Add(new FieldDefUser("field", new FieldSig(module.CorLibTypes.Int32)));
    if (collision) module.Types.Add(new TypeDefUser("", "O", module.CorLibTypes.Object.TypeDefOrRef));
    if (unexpected) { module.Types.Add(new TypeDefUser("", "Other", module.CorLibTypes.Object.TypeDefOrRef)); module.Types.Add(new TypeDefUser("", "Other", module.CorLibTypes.Object.TypeDefOrRef)); }
    var keep = new TypeDefUser("Fixture", "Keep", module.CorLibTypes.Object.TypeDefOrRef); module.Types.Add(keep);
    var method = new MethodDefUser("Value", MethodSig.CreateStatic(module.CorLibTypes.Int32)) { Attributes = dnlib.DotNet.MethodAttributes.Public | dnlib.DotNet.MethodAttributes.Static, Body = new CilBody() };
    method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, 37)); method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret)); keep.Methods.Add(method);
    using var stream = new MemoryStream(); module.Write(stream); return stream.ToArray();
}
if (args.Length == 3 && args[0] == "--probe") {
    byte[] before = File.ReadAllBytes(args[1]);
    Check(!Loads(before), "supplied original rejected by managed loader");
    var result = MetadataRepair.Prepare(before);
    Check(result.RenamedTypes == 3, "three captured empty helper names repaired");
    Check(result.Bytes.Length == before.Length, "file size preserved");
    Check(before.Zip(result.Bytes).Count(p => p.First != p.Second) == 3, "only three bytes differ in the captured file");
    Check(Loads(result.Bytes, "UnityEngine.LogType"), "repaired supplied CoreModule and LogType load");
    Check(MetadataRepair.Prepare(result.Bytes).RenamedTypes == 0, "second pass is a no-op");
    File.WriteAllBytes(args[2], result.Bytes);
    Console.WriteLine("INPUT_SHA256=" + MetadataRepair.Sha256(before)); Console.WriteLine("OUTPUT_SHA256=" + MetadataRepair.Sha256(result.Bytes));
    Console.WriteLine($"ALL {checks} PRIVATE METADATA CHECKS PASSED; NOT A GAME TEST"); return;
}
var original = Fixture();
var repaired = MetadataRepair.Prepare(original, false);
Check(repaired.RenamedTypes == 3, "exactly three names renamed");
Check(repaired.Bytes.Length == original.Length, "file size preserved");
Check(!Loads(original), "original duplicate fixture is rejected");
Check(Loads(repaired.Bytes, "Fixture.Keep"), "repaired fixture loads");
using (var before = ModuleDefMD.Load(original)) using (var after = ModuleDefMD.Load(repaired.Bytes)) {
    Check(before.GetTypes().Count() == after.GetTypes().Count(), "TypeDef count unchanged");
    Check(before.GetTypes().Sum(t => t.Fields.Count) == after.GetTypes().Sum(t => t.Fields.Count), "FieldDef count unchanged");
    Check(before.GetTypes().Sum(t => t.Methods.Count) == after.GetTypes().Sum(t => t.Methods.Count), "MethodDef count unchanged");
    Check(after.GetTypes().GroupBy(t => t.FullName).All(g => g.Count() == 1), "no duplicate full names remain");
    Check(after.GetTypes().Any(t => t.Name == "<>O") && after.GetTypes().Any(t => t.Name == "<>c"), "original anchor names retained");
    Check(before.GetTypes().Last().Methods[0].Body.Instructions.Select(i=>i.ToString()).SequenceEqual(after.GetTypes().Last().Methods[0].Body.Instructions.Select(i=>i.ToString())), "method instructions unchanged");
}
var again = MetadataRepair.Prepare(repaired.Bytes, false);
Check(again.RenamedTypes == 0 && again.Bytes.SequenceEqual(repaired.Bytes), "repeat pass is byte-for-byte no-op");
Refuses(()=>MetadataRepair.Prepare(original), "uncaptured production layout rejected");
Refuses(()=>MetadataRepair.Prepare(Fixture(field:true),false), "nonempty helper rejected");
Refuses(()=>MetadataRepair.Prepare(Fixture(collision:true),false), "replacement-name collision rejected");
Refuses(()=>MetadataRepair.Prepare(Fixture(unexpected:true),false), "unexpected duplicate group rejected");
Refuses(()=>MetadataRepair.Prepare(Fixture(name:"OtherAssembly"),false), "wrong assembly rejected");
Refuses(()=>MetadataRepair.Prepare(new byte[]{1,2,3}), "invalid file rejected");
string dir = Path.Combine(Path.GetTempPath(), "Mimic Party (bootstrap tests) " + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(dir);
try {
    string target = Path.Combine(dir,"UnityEngine.CoreModule.dll"); File.WriteAllBytes(target,original);
    var fileResult = MetadataRepair.RepairFile(target,false);
    Check(File.ReadAllBytes(target).SequenceEqual(repaired.Bytes), "transaction installs validated bytes");
    var backups = Directory.GetFiles(Path.Combine(dir,".arribbaa-backups"));
    Check(backups.Length == 1 && File.ReadAllBytes(backups[0]).SequenceEqual(original), "original backup preserved");
    Check(MetadataRepair.RepairFile(target,false).RenamedTypes==0, "installed file idempotent");
    Check(Directory.GetFiles(dir,"*.repair-*").Length==0, "temporary files removed");
    File.WriteAllBytes(target,original); File.WriteAllText(backups[0],"conflicting backup");
    Refuses(()=>MetadataRepair.RepairFile(target,false), "conflicting backup rejected");
    Check(File.ReadAllBytes(target).SequenceEqual(original), "failure leaves target unchanged");
} finally { Directory.Delete(dir,true); }
Console.WriteLine($"ALL {checks} BOOTSTRAP REGRESSION CHECKS PASSED");
