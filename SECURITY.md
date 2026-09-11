# Security and file behaviour

The public Core download contains two ordinary managed DLLs and documentation. It contains no EXE, BAT, CMD, PowerShell installer, password-protected archive, nested archive, private log or game/Unity binary. Source is available for inspection; the components are not obfuscated or packed.

The Core supports runtime memory patching when a feature mod requests it. Pattern matching and expected-byte verification are used before writes, with transactional rollback. Runtime modifications can resemble behaviour heuristically flagged by antivirus tools. A hash proves file identity, not safety or moderation approval.

The Interop Bootstrap checks the game's SHA-256 fingerprints and the generated assembly structure. It changes only three TypeDef name indexes in the recognized malformed generated CoreModule, saves an original backup, and leaves an already-valid file unchanged. It may remove a hash-recognized obsolete CoreModule copy from BepInEx/core after backing it up. It disables UnityLogListening through the BepInEx configuration API. It does not replace native game binaries, scan unrelated folders, upload data, add an updater or change Windows/antivirus settings.

The Core and bootstrap have no networking or telemetry. BepInEx itself can download Unity reference libraries during initial generation; this is separate upstream behaviour.

Download only from the author's GitHub Releases or official Nexus listings. Compare SHA256SUMS.txt if checking file identity. If a security product reports a detection, retain the filename, SHA-256 and detection details and request review from the vendor/Nexus. Do not disable security protection merely to install the mod. A GitHub release is not a substitute for Nexus security approval.

Report suspected vulnerabilities privately through the author's Nexus contact rather than posting private logs or binaries. General reproducible bugs can go to GitHub Issues with personal data removed.
