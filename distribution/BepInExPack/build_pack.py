"""Release packaging only. Never executed on a player's computer."""
from pathlib import Path, PurePosixPath
import hashlib, json, zipfile, xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
INPUT = ROOT / '_pack'
OUT = ROOT / 'pack-dist'
OUT.mkdir(exist_ok=True)
H = lambda b: hashlib.sha256(b).hexdigest()
J = lambda obj: (json.dumps(obj, indent=2, ensure_ascii=False) + '\n').encode('utf-8')
TAG_URL = 'https://github.com/LocoPablito/MimicParty-Modding/releases/tag/bepinex-pack-v1.0.0'

def readzip(path):
    with zipfile.ZipFile(path) as z:
        assert z.testzip() is None
        result = {}
        for entry in z.infolist():
            if entry.is_dir(): continue
            n = entry.filename.replace('\\', '/')
            if n.startswith('/') or '..' in PurePosixPath(n).parts: raise ValueError('Unsafe path')
            if n in result: raise ValueError('Duplicate archive path')
            result[n] = z.read(entry)
        return result

def writezip(path, files):
    names = [n.casefold() for n in files]
    if len(set(names)) != len(names): raise ValueError('Case-insensitive collision')
    with zipfile.ZipFile(path, 'w', zipfile.ZIP_DEFLATED, compresslevel=6) as z:
        for name, data in sorted(files.items()):
            info = zipfile.ZipInfo(name, (2026, 9, 11, 0, 0, 0))
            info.compress_type = zipfile.ZIP_DEFLATED
            info.external_attr = 0o100644 << 16
            z.writestr(info, data)
    assert readzip(path) == files

def manifest(files):
    return ''.join(f'{H(b)}  {n}\n' for n,b in sorted(files.items())).encode()

raw_path = INPUT / 'raw/BepInEx_official_788.zip'
raw = raw_path.read_bytes()
assert H(raw) == 'f4cc496bd098a0df4164b81e3737297707f13a47c2478dba2f60eefab784817a'
original = readzip(raw_path)
assert len(original) == 228
core = readzip(INPUT / 'core/MimicParty_Modding_Core_v1.0.0.zip')
bootstrap = core['BepInEx/patchers/MimicParty.InteropBootstrap.dll']
assert H(bootstrap) == '5e8408516988c28bd5e0f54d859877dc0939ce5e0a0d6bbae2e3b33c33e82f01'
assert H(core['BepInEx/plugins/MimicPartyModdingCore.dll']) == '4ebd390eb5a587999a34919cea2309d7d81bffd992816971c54e07553727b87d'
files = dict(original)
files['BepInEx/patchers/MimicParty.InteropBootstrap.dll'] = bootstrap
files['BepInEx/config/BepInEx.cfg'] = b'[Logging]\nUnityLogListening = false\n\n[Logging.Console]\nEnabled = false\nPreventClose = true\n\n[Logging.Disk]\nEnabled = true\n\n[IL2CPP]\nUpdateInteropAssemblies = true\n'
DOC = 'BepInExPack/'
files['README_BEPINEX_PACK.txt'] = '''BepInEx Pack for Mimic Party 1.0.0
Pack configuration and maintenance: arribbaa
Loader: BepInEx team and contributors

WHAT THIS IS
Official BepInEx 6.0.0-be.788 Unity IL2CPP Windows x64, kept byte-for-byte
unchanged, plus a Mimic Party configuration and Interop Bootstrap 1.0.0.
The pack includes the loader and its embedded runtime. Do not install another
BepInEx download on top of this pack. It does NOT include Modding Core or the
10 Player Expansion and makes no gameplay or lobby-capacity change by itself.

DOCUMENTED TARGET
Mimic Party v0.1.73, Windows x64 / Steam, Unity 6000.4.2f1.
Only the captured game/metadata fingerprints are accepted for the generated
CoreModule compatibility repair. No future-update compatibility is promised.
BepInEx #788 is an upstream bleeding-edge build, not an LTS/security guarantee.
Its original embedded .NET runtime is 6.0.7 and is not replaced by this pack.

INSTALL
1. Close Mimic Party. Open Steam > Mimic Party > Manage > Browse local files.
2. Extract this ZIP into the folder containing Mimic Party.exe. The file
   winhttp.dll, doorstop_config.ini, dotnet/ and BepInEx/ belong in that root.
3. Install the Core package if your chosen mod requires it; then install that
   mod. Expansion Complete already includes Core. Install these BEFORE launch.
4. Start the game normally through Steam. Initial generation can take longer
   and can download required reference libraries. Wait; do not interrupt it.

ALREADY HAVE BEPINEX?
Do not blindly overwrite an unrelated winhttp.dll, a different loader or your
custom BepInEx.cfg. Back up the existing loader/configuration outside the game
folder first. When migrating from this project's known build 788, keep your
plugins and generated interop cache. The supplied config is a fresh-install
preset; existing users can retain their cfg and apply its settings manually.
Core 1.0.0 / Expansion Complete contain the same bootstrap DLL at the same
path. An identical file may be replaced, but never create renamed duplicates.

PRESET / LOGGING
No console window by default. Disk logging remains enabled.
To show the console, set Enabled = true under [Logging.Console] in
BepInEx/config/BepInEx.cfg and restart. Minimize the console instead of closing
it while the game is running. Do not disable antivirus to install this pack.

COMPATIBILITY COMPONENT
The bootstrap checks game fingerprints and the generated assembly layout,
repairs the recognized duplicate empty helper type names, and backs up the
original locally. Already-valid files are not rewritten. It does not include
UnityEngine.CoreModule.dll, GameAssembly.dll, game metadata or personal logs.

CHECK THE FIRST RUN
Inspect BepInEx/LogOutput.log for INTEROP COMPATIBILITY READY and
Chainloader startup complete. Core/feature plugin messages appear only if
those separate components were installed. This pack has no in-game menu.

TEST SCOPE
All 228 upstream files are verified unchanged. The bootstrap is the previously
built component with 23 Windows regression checks and six private supplied-
assembly metadata/load checks. The earlier Core/Expansion Windows session
used an already-repaired installation; a fresh Windows game launch of this
new pack is NOT yet recorded. Packaging checks are not a game-runtime test.

REMOVE / RECOVER
Close the game. To disable this loader, set enabled = false in [General] of
doorstop_config.ini, then restart. This disables all mods using this loader.
For complete removal, back up any configurations you want to keep, then remove
only loader-owned paths identified in the included manifest. Do not delete
another mod's files or the game's own DLLs. Restore your pre-install backups
when reverting an existing loader installation.

LICENSING / SOURCE
BepInEx and UnityDoorstop: LGPL 2.1. Il2CppInterop: LGPL 3.0 (GPL included).
Other components retain their included individual licenses. arribbaa does not
claim ownership of those projects. See BepInExPack/licenses and SOURCE_ACCESS.
The separate SOURCES ZIP is for review/rebuilding, not game installation.
No installer scripts, telemetry or updater were added. BepInEx's own first-run
reference-library downloads are normal required upstream behaviour.
'''.encode()
files[DOC+'CHANGELOG.txt'] = b'1.0.0 - 11 September 2026\nInitial Mimic Party-specific pack.\nPreserves all 228 files from official BepInEx build 788.\nAdds the existing Interop Bootstrap 1.0.0 and a hidden-console, disk-log-enabled preset.\nAdds license notices, source access, checksums and installation documentation.\nDoes not bundle Modding Core, Expansion or game-generated assemblies.\nFresh-install game validation remains separate from package verification.\n'
files[DOC+'AUTHOR_COMPONENT_LICENSE.txt'] = core['LICENSE.txt'] + b'\nAdditional permission for LGPL interoperability: private modification and reverse engineering needed to debug/relink this component with interface-compatible modified LGPL dependencies are permitted. No restriction here narrows rights granted by an upstream license. This addition does not authorize third-party rebranding of the Core/Bootstrap.\n'

# Map shipped managed libraries to the exact NuGet bytes, not guessed versions.
dlls={n:H(b) for n,b in original.items() if n.endswith('.dll')}
byname={PurePosixPath(n).name:(n,h) for n,h in dlls.items()}
records=[]
for package in sorted((INPUT/'review/nuget').glob('*.nupkg')):
    entries=readzip(package)
    matched=[]
    for n,b in entries.items():
        pair=byname.get(PurePosixPath(n).name)
        if pair and H(b)==pair[1]: matched.append(pair[0])
    if not matched: continue
    spec=next(b for n,b in entries.items() if n.endswith('.nuspec'))
    xml=ET.fromstring(spec)
    meta=next(x for x in xml if x.tag.endswith('metadata'))
    item={x.tag.split('}')[-1]:x.text for x in meta if x.tag.split('}')[-1] in ['id','version','authors','copyright','license','licenseUrl','projectUrl']}
    for x in meta:
        if x.tag.split('}')[-1]=='repository': item['repository']=x.attrib
    item['files']=sorted(set(matched));records.append(item)
    for n,b in entries.items():
        if any(word in PurePosixPath(n).name.lower() for word in ['license','notice','copying','copyright']):
            files[DOC+'licenses/NuGet/'+item['id']+'/'+n]=b
    files[DOC+'licenses/NuGet/'+item['id']+'/package.nuspec']=spec

source_archives={'BepInEx':INPUT/'raw/BepInEx_788_source.zip'}
source_archives.update({p.stem:p for p in (INPUT/'legal').glob('*.zip')})
source_archives.update({p.stem:p for p in (INPUT/'deps').glob('*.zip')})
source_origins={
    'BepInEx':{'Repository':'BepInEx/BepInEx','Revision':'5b766a3b7f6c164d4798924a93f3acf4db769d06','URL':'https://github.com/BepInEx/BepInEx/tree/5b766a3b7f6c164d4798924a93f3acf4db769d06'}
}
for folder in ['legal','deps']:
    source_origins.update(json.loads((INPUT/folder/'SOURCE_ORIGINS.json').read_text()))
for name,path in source_archives.items():
    with zipfile.ZipFile(path) as z:
        for n in z.namelist():
            rel='/'.join(n.split('/')[1:])
            if not rel or n.endswith('/'): continue
            if any(word in PurePosixPath(rel).name.lower() for word in ['license','notice','copying','copyright']):
                if name.startswith('dotnet-runtime-build') and '/' in rel: continue
                if len(rel)>220: continue
                files[DOC+'licenses/Upstream/'+name+'/'+rel]=z.read(n)
for name in ['DOTNET_LICENSE.TXT','DOTNET_THIRD-PARTY-NOTICES.TXT']:
    files[DOC+'licenses/'+name]=(INPUT/'legal'/name).read_bytes()
assert any('Il2CppInterop' in n and n.endswith('/LICENSE') for n in files)
assert any('UnityDoorstop' in n and n.endswith('/LICENSE') for n in files)
assert any('BepInEx/LICENSE' in n for n in files)
files[DOC+'DEPENDENCIES.json']=J(records)
files[DOC+'SOURCE_ORIGINS.json']=J(source_origins)
files[DOC+'THIRD_PARTY_NOTICES.txt']=('Pack maintained by arribbaa. Upstream rights are unchanged.\n\nBepInEx 6.0.0-be.788: BepInEx team/contributors, LGPL-2.1.\nUnityDoorstop 4.5.0: NeighTools/contributors, LGPL-2.1.\nIl2CppInterop 1.5.3: knah, BepInEx et al., LGPL-3.0-only.\nDobby 1.0.5: Dobby authors/contributors, Apache-2.0.\n.NET 6.0.7: .NET Foundation/contributors, MIT plus included notices.\n\nExact package-matched managed dependencies:\n'+''.join(f"{r['id']} {r['version']} | {r.get('authors','')} | {r.get('license') or r.get('licenseUrl')}\n{r.get('copyright') or ''}\n" for r in records)+'\nIndividual texts and additional credits are retained in licenses/. No author permission setting may override upstream rights. Source/relinking access is provided alongside the runtime download.\n').encode()
source_access=f'''Source and license access

Runtime release and corresponding SOURCES download: {TAG_URL}
On Nexus, upload the SOURCES ZIP on the SAME BepInEx Pack page under Miscellaneous.
The user does not need to install the SOURCES download to play.

The source bundle contains the source/build files for BepInEx build 788,
Il2CppInterop 1.5.3, UnityDoorstop 4.5.0, Dobby 1.0.5 and the exact Bootstrap
component. BepInEx's three prebuilt Unity reference DLLs are not redistributed;
obtain the upstream reference dependencies as described by that project's build
configuration. They are not the source of the distributed LGPL libraries.
Other dependency source locations and exact revisions are recorded in SOURCE_ORIGINS.
.NET and permissively licensed dependencies retain their own included notices.

The published Bootstrap source revision is c6984654398c69f4161cfe7b7fb36c1af09f7091.
Upstream binaries in this runtime ZIP have NOT been modified or renamed.
The LGPL libraries are separate DLLs and are not combined into an obfuscated binary.
Modification/relinking and debugging rights granted by those licenses are retained.
'''
files[DOC+'SOURCE_ACCESS.txt']=source_access.encode()
prov={
 'Name':'BepInEx Pack for Mimic Party','PackVersion':'1.0.0','Maintainer':'arribbaa',
 'UpstreamArchiveSHA256':H(raw),'UpstreamVersion':'6.0.0-be.788','UpstreamFileCount':228,
 'UpstreamFilesUnchanged':True,'BootstrapSHA256':H(bootstrap),'BootstrapVersion':'1.0.0',
 'GameAssemblySHA256':'44bbc82bdae73c1c86559a1f091ee9c7a3ae510a02c2d83f16b686ecdd9c8b11',
 'MetadataSHA256':'1586b9dd69e488706671d35490cc16377a612ca3af521031ac44464929b21e94',
 'EmbeddedRuntime':'6.0.7','IncludesCore':False,'IncludesExpansion':False,'IncludesGameAssemblies':False,
 'FreshInstallGameTest':False,'BootstrapWindowsRegressionChecks':23,'BootstrapSuppliedMetadataChecks':6,
 'RuntimeDownloadHasInstallerScripts':False,'ThirdPartyLicenseTextsIncluded':True,
 'MatchedNuGetPackages':len(records),'PublishedAtNexus':False
}
files[DOC+'PROVENANCE.json']=J(prov)
files[DOC+'UPSTREAM_SHA256SUMS.txt']=manifest(original)
assert all(files[n]==b for n,b in original.items())
assert 'BepInEx/plugins/MimicPartyModdingCore.dll' not in files
assert 'BepInEx/plugins/MimicParty10PlayerExpansion.dll' not in files
assert not any(PurePosixPath(n).suffix.lower() in {'.exe','.bat','.cmd','.ps1','.zip','.rar','.7z','.ttf','.otf','.woff','.woff2','.ttc'} for n in files)
assert not any(PurePosixPath(n).name.lower() in {'gameassembly.dll','unityengine.coremodule.dll','global-metadata.dat','unityplayer.dll'} for n in files)
files[DOC+'SHA256SUMS.txt']=manifest(files)
pack=OUT/'BepInEx_Pack_for_Mimic_Party_v1.0.0.zip'
writezip(pack,files)

# Preferred-form source for the LGPL components, plus the bootstrap source.
# Omit prebuilt references/test binaries and fonts; never distribute a game DLL.
sources={'README_SOURCE_ACCESS.txt':source_access.encode()}
skips=[]
binary_suffixes={'.dll','.exe','.pdb','.so','.dylib','.lib','.a','.zip','.nupkg','.snupkg','.7z','.rar','.ttf','.otf','.woff','.woff2','.ttc','.eot'}
for name in ['BepInEx','Il2CppInterop_1.5.3','UnityDoorstop_4.5.0','Dobby_1.0.5']:
    with zipfile.ZipFile(source_archives[name]) as z:
        for n in z.namelist():
            rel='/'.join(n.split('/')[1:])
            if not rel or n.endswith('/'): continue
            if PurePosixPath(rel).suffix.lower() in binary_suffixes:
                skips.append(name+'/'+rel);continue
            sources[name+'/'+rel]=z.read(n)
for p in sorted((ROOT/'src/MimicParty.InteropBootstrap').glob('*')):
    if p.is_file() and p.suffix in {'.cs','.csproj'}:sources['MimicParty.InteropBootstrap/'+p.name]=p.read_bytes()
sources['MimicParty.InteropBootstrap/LICENSE.txt']=files[DOC+'AUTHOR_COMPONENT_LICENSE.txt']
sources['SOURCE_ORIGINS.json']=J(source_origins)
sources['EXCLUDED_PREBUILT_FILES.txt']=('\n'.join(skips)+'\n').encode()
for n,b in files.items():
    if n.startswith(DOC+'licenses/') or n.endswith('DEPENDENCIES.json'): sources[n]=b
sources['SHA256SUMS.txt']=manifest(sources)
source_zip=OUT/'BepInEx_Pack_for_Mimic_Party_v1.0.0_SOURCES.zip'
writezip(source_zip,sources)

starter={p.name:p.read_bytes() for p in (ROOT/'examples/StarterMod').iterdir() if p.is_file() and p.suffix in {'.cs','.csproj','.md','.txt'}}
assert {'Plugin.cs','StarterMod.csproj','README.md','LICENSE.txt'} <= starter.keys()
starter['BUILD_VALIDATION.txt']=b'Compiled against the released Core 1.0.0 on Windows.\nWorkflow run: 34633663729; source: d7a479877e38481fde6a977362bd53a1e1239a5a.\nNo Core DLL was copied into the starter output. Not an in-game feature test.\n'
starter['SHA256SUMS.txt']=manifest(starter)
starter_zip=OUT/'MimicParty_Core_Developer_Starter_v1.0.0.zip'
writezip(starter_zip,starter)
all_outputs=[pack,source_zip,starter_zip]
(OUT/'SHA256SUMS.txt').write_bytes(''.join(f'{H(p.read_bytes())}  {p.name}\n' for p in all_outputs).encode())
(OUT/'PACK_VALIDATION.json').write_bytes(J({'Result':'PACKAGE_VALIDATED_NOT_FRESH_GAME_TESTED','UpstreamFilesChecked':len(original),'RuntimeArchiveFiles':len(files),'MatchedNuGetPackages':len(records),'SourceFiles':len(sources),'ExcludedPrebuiltSourceFiles':len(skips),'StarterCompiled':True,'CoreRuntimeUnchanged':True,'BootstrapUnchanged':True,'GameFilesIncluded':False,'FreshGameTest':False}))
print((OUT/'PACK_VALIDATION.json').read_text())
print((OUT/'SHA256SUMS.txt').read_text())
