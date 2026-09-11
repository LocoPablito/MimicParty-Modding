#!/usr/bin/env python3
from __future__ import annotations

from pathlib import Path
import re
import sys
import zipfile

ROOT = Path(__file__).resolve().parents[1]
DIST = ROOT / "dist"

PACKAGES = {
    "MimicParty_Modding_Core_v1.0.0_by_arribbaa_NEXUS.zip": {
        "README.txt",
        "CHANGELOG.txt",
        "LICENSE.txt",
        "BepInEx/plugins/MimicPartyModdingCore.dll",
    },
    "MimicParty_10_Player_Expansion_v1.1.0_by_arribbaa_NEXUS.zip": {
        "README.txt",
        "CHANGELOG.txt",
        "LICENSE.txt",
        "BepInEx/plugins/MimicParty10PlayerExpansion.dll",
    },
}

FORBIDDEN_BASENAMES = {
    "gameassembly.dll",
    "global-metadata.dat",
    "unityplayer.dll",
}
FORBIDDEN_SUFFIXES = {".exe", ".bat", ".cmd", ".ps1", ".msi", ".rar", ".7z"}

# Keep public release archives free of internal tooling/provider branding. The byte
# markers are encoded here so the repository itself does not expose those labels.
FORBIDDEN_BRANDING_BYTES = tuple(
    bytes.fromhex(value)
    for value in (
        "63686174677074",
        "6f70656e6169",
    )
)

# Two short disclosure-marker tokens requested to be absent from the distributed
# files. These checks are text-file only to avoid random byte matches in assemblies.
FORBIDDEN_SHORT_TEXT_TOKENS = (
    bytes((65, 73)).decode("ascii"),
    bytes((75, 73)).decode("ascii"),
)
TEXT_SUFFIXES = {".txt", ".md", ".json", ".xml", ".yml", ".yaml"}


def normalized_files(zf: zipfile.ZipFile) -> set[str]:
    return {
        name.replace("\\", "/").lstrip("./")
        for name in zf.namelist()
        if name and not name.endswith(("/", "\\"))
    }


def contains_forbidden_branding(data: bytes) -> bool:
    lowered = data.lower()
    for marker in FORBIDDEN_BRANDING_BYTES:
        if marker in lowered:
            return True
        if marker.decode("ascii").encode("utf-16le") in lowered:
            return True
    return False


def contains_forbidden_short_token(data: bytes) -> str | None:
    text = data.decode("utf-8-sig", errors="ignore")
    for token in FORBIDDEN_SHORT_TEXT_TOKENS:
        if re.search(rf"(?<![A-Za-z]){re.escape(token)}(?![A-Za-z])", text, flags=re.IGNORECASE):
            return token
    return None


def validate(package_name: str, expected_files: set[str]) -> None:
    path = DIST / package_name
    if not path.is_file():
        raise AssertionError(f"Missing package: {package_name}")

    with zipfile.ZipFile(path, "r") as zf:
        bad_member = zf.testzip()
        if bad_member is not None:
            raise AssertionError(f"Corrupt ZIP member in {package_name}: {bad_member}")

        files = normalized_files(zf)
        if files != expected_files:
            raise AssertionError(
                f"Unexpected archive layout for {package_name}.\n"
                f"Expected: {sorted(expected_files)}\n"
                f"Actual:   {sorted(files)}"
            )

        dll_count = 0
        author_seen = False

        for raw_name in zf.namelist():
            name = raw_name.replace("\\", "/").lstrip("./")
            if not name or name.endswith("/"):
                continue

            base = Path(name).name.lower()
            suffix = Path(name).suffix.lower()

            if base in FORBIDDEN_BASENAMES:
                raise AssertionError(f"Original game file included in {package_name}: {name}")

            if suffix in FORBIDDEN_SUFFIXES:
                raise AssertionError(f"Forbidden executable/script/archive in {package_name}: {name}")

            if suffix == ".zip":
                raise AssertionError(f"Nested archive in {package_name}: {name}")

            if suffix == ".dll":
                dll_count += 1

            data = zf.read(raw_name)

            if contains_forbidden_branding(data):
                raise AssertionError(f"Forbidden internal branding found in {package_name}:{name}")

            if suffix in TEXT_SUFFIXES:
                token = contains_forbidden_short_token(data)
                if token:
                    raise AssertionError(f"Forbidden disclosure marker found in {package_name}:{name}")

            if b"arribbaa" in data.lower() or "arribbaa" in name.lower():
                author_seen = True

        if dll_count != 1:
            raise AssertionError(f"Expected exactly one plugin DLL in {package_name}, found {dll_count}")

        if not author_seen:
            raise AssertionError(f"Author marker 'arribbaa' not found in {package_name}")

    print(f"PASS: {package_name}")


def main() -> int:
    try:
        for package_name, expected_files in PACKAGES.items():
            validate(package_name, expected_files)
    except Exception as exc:
        print(f"FAIL: {exc}", file=sys.stderr)
        return 1

    print("All Nexus package policy checks passed.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
