# Building

Use the .NET 8 SDK. The Core targets net6.0 to match the pinned loader.

```powershell
./build.ps1
```

The optional Developer Starter uses the installed Core DLL; follow examples/StarterMod/README.md.

Publication extracts the pinned runtime bytes recorded in release.json instead of silently substituting a rebuild. Run `python scripts/package_release.py` with network access to reproduce the R2 archives from those inputs and the current documents. Different documentation changes archive hashes; the runtime hash is checked separately.

The first-party runtime source remains inspectable. Private development captures and obsolete repair tools are not distribution inputs.
