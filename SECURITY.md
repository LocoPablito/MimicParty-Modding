# Security and support

Obtain files from the official Nexus page or this repository's releases. SHA-256 manifests identify bytes; they do not certify safety or antivirus approval. Request review of unexpected detections rather than disabling protection.

The Core exposes native runtime patching to dependent mods. It checks unique signatures and expected bytes before applying a transaction and attempts matching-byte rollback. It does not sandbox another mod or validate arbitrary offsets for its author. Only load trusted feature mods.

The Core has no telemetry, updater or downloader. The separate BepInEx Pack has its own documented file and network behaviour. Do not post private logs, account identifiers or game binaries publicly. Include versions and a sanitized error excerpt when reporting an issue.
