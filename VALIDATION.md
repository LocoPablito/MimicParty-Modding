# Validation scope

## Core runtime 1.0.0

An actual Windows game run on the documented build loaded Core 1.0.0 and Expansion 1.1.2, completed the chainloader and passed all 13 collected loader/capacity checks. The released Core runtime SHA-256 is `4ebd390eb5a587999a34919cea2309d7d81bffd992816971c54e07553727b87d`.

## Interop Bootstrap 1.0.0

This is a separate compatibility component for first-time installations, not part of that earlier Windows game session. Its automated tests cover duplicate-layout recognition, preservation of metadata and method contents, refusal of unknown/nonempty/conflicting inputs, idempotency, backups and transactional file replacement. The captured original generated assembly is also checked privately with a .NET 6.0.7 loader; no game files are committed to this repository.

The underlying repair is a three-byte change to TypeDef name indexes. A full fresh-install Windows game launch using this automatic bootstrap is not established by the earlier Core/Expansion test. No ten-client end-to-end test is claimed. See the workflow results and release provenance for the exact artifacts.
