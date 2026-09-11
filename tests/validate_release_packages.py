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
FORBIDDEN_TEXT = ("chatgpt", "openai")


def normalized_files(zf: zipfile.ZipFile) -> set[str]:
    return {
        name.replace("\\", "/").lstrip("./")
        for name in zf.namelist()
        if name and not name.endswith(("/", "\\"))
    }


def contains_forbidden_text(data: bytes) -> str | None:
    lowered = data.lower()
    for term in FORBIDDEN_TEXT:
        if term.encode("ascii") in lowered:
            return term
        if term.encode("utf-16le") in lowered:
            return term
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
            forbidden = contains_forbidden_text(data)
            if forbidden:
                raise AssertionError(f"Forbidden text '{forbidden}' found in {package_name}:{name}")

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
