"""Package pinned, previously validated binaries; never rebuild the Core runtime."""
from pathlib import Path
import hashlib
import io
import json
import zipfile

ROOT = Path(__file__).resolve().parents[1]
sha = lambda b: hashlib.sha256(b).hexdigest()

def dll_from_artifact(directory, filename):
    matches = []
    for path in sorted(directory.rglob('*.zip')):
        with zipfile.ZipFile(path) as z:
            for name in z.namelist():
                if name.replace('\\', '/').split('/')[-1] == filename:
                    matches.append(z.read(name))
    if len(matches) != 1:
        raise RuntimeError(f'Expected one {filename}; found {len(matches)}')
    return matches[0]

core = dll_from_artifact(ROOT / '_release-input/core', 'MimicPartyModdingCore.dll')
boot = (ROOT / '_release-input/bootstrap/MimicParty.InteropBootstrap.dll').read_bytes()
assert sha(core) == '4ebd390eb5a587999a34919cea2309d7d81bffd992816971c54e07553727b87d'
assert sha(boot) == '5e8408516988c28bd5e0f54d859877dc0939ce5e0a0d6bbae2e3b33c33e82f01'
files = {
    'BepInEx/plugins/MimicPartyModdingCore.dll': core,
    'BepInEx/patchers/MimicParty.InteropBootstrap.dll': boot,
}
for name in ['README.md', 'CHANGELOG.md', 'LICENSE.txt', 'SECURITY.md', 'SUPPORTED_BUILD.md', 'VALIDATION.md']:
    files[name] = (ROOT / name).read_bytes()
provenance = {
    'Package': 'Mimic Party Modding Core', 'Version': '1.0.0', 'Author': 'arribbaa',
    'Core': {'Version': '1.0.0', 'SHA256': sha(core), 'SourceCommit': '46c03672bab67ed32a060198ec8f2d56d3fdfd6a', 'WorkflowRun': 34606013538},
    'InteropBootstrap': {'Version': '1.0.0', 'SHA256': sha(boot), 'SourceCommit': 'c6984654398c69f4161cfe7b7fb36c1af09f7091', 'WorkflowRun': 34628163123},
    'BepInExIncluded': False, 'GameBinariesIncluded': False,
    'CoreWindowsRuntimeVerified': True, 'AutomaticBootstrapFreshGameVerified': False,
    'BootstrapRegressionChecks': 23, 'BootstrapPrivateMetadataChecks': 6,
}
files['PROVENANCE.json'] = (json.dumps(provenance, indent=2) + '\n').encode()
files['SHA256SUMS.txt'] = ''.join(f'{sha(b)}  {n}\n' for n,b in sorted(files.items())).encode()
out = ROOT / 'dist'; out.mkdir(exist_ok=True)
archive = out / 'MimicParty_Modding_Core_v1.0.0.zip'
with zipfile.ZipFile(archive, 'w', zipfile.ZIP_DEFLATED, compresslevel=9) as z:
    for name, data in sorted(files.items()):
        info = zipfile.ZipInfo(name, (2026,9,11,0,0,0)); info.compress_type = zipfile.ZIP_DEFLATED
        info.external_attr = 0o100644 << 16
        z.writestr(info, data)
with zipfile.ZipFile(archive) as z:
    assert z.testzip() is None
    assert set(z.namelist()) == set(files)
    assert not any(Path(n).suffix.lower() in {'.exe','.bat','.cmd','.ps1','.zip','.7z','.rar'} for n in z.namelist())
    assert not any(Path(n).name.lower() in {'gameassembly.dll','unityengine.coremodule.dll','global-metadata.dat','unityplayer.dll'} for n in z.namelist())
(out / 'SHA256SUMS.txt').write_text(f'{sha(archive.read_bytes())}  {archive.name}\n', encoding='ascii')
print(f'Package validation PASS: {archive.name}; {sha(archive.read_bytes())}')
