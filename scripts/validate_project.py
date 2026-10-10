#!/usr/bin/env python3
"""Fast repository sanity checks. This does not replace a Unity Editor compile or device test."""
from __future__ import annotations

import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
EXPECTED_EDITOR = "6000.3.26f1"

REQUIRED_FILES = [
    "README.md",
    "Packages/manifest.json",
    "ProjectSettings/ProjectVersion.txt",
    "Assets/Editor/ProjectBootstrapper.cs",
    "Assets/Scripts/Core/GameManager.cs",
    "Assets/Scripts/Core/CameraFollow.cs",
    "Assets/Scripts/Player/PlayerController.cs",
    "Assets/Scripts/Gameplay/FoodSpawner.cs",
    "Assets/Scripts/Gameplay/FoodItem.cs",
    "Assets/Scripts/Gameplay/WormController.cs",
    "Assets/Scripts/Gameplay/WormSegment.cs",
    "Assets/Scripts/Gameplay/WormAIController.cs",
    "Assets/Scripts/UI/ScoreManager.cs",
    "Assets/Scripts/UI/ScoreHUD.cs",
    "Assets/Scripts/UI/GameFlowUI.cs",
    "docs/RESEARCH.md",
    "docs/GAMEPLAY.md",
    "docs/UNITY_SETUP.md",
    "docs/ROADMAP.md",
]

def fail(message: str) -> None:
    print(f"ERROR: {message}", file=sys.stderr)
    raise SystemExit(1)

def main() -> None:
    missing = [p for p in REQUIRED_FILES if not (ROOT / p).is_file()]
    if missing:
        fail("Missing required files: " + ", ".join(missing))

    try:
        manifest = json.loads((ROOT / "Packages/manifest.json").read_text(encoding="utf-8"))
    except (OSError, json.JSONDecodeError) as exc:
        fail(f"Packages/manifest.json is not valid JSON: {exc}")

    dependencies = manifest.get("dependencies", {})
    if dependencies.get("com.unity.inputsystem") != "1.20.1":
        fail("Input System should remain pinned to the validated Unity 6000.3 version 1.20.1.")

    version_text = (ROOT / "ProjectSettings/ProjectVersion.txt").read_text(encoding="utf-8")
    if f"m_EditorVersion: {EXPECTED_EDITOR}" not in version_text:
        fail(f"ProjectVersion.txt must pin Editor {EXPECTED_EDITOR}.")
    if "de8167e7440c" not in version_text:
        fail("ProjectVersion.txt changeset does not match the pinned Editor patch.")

    generator = (ROOT / "Assets/Editor/ProjectBootstrapper.cs").read_text(encoding="utf-8")
    if 'MenuItem("Tools/IO/Create-Reset Demo Scene and Prefabs")' not in generator:
        fail("Unity Editor bootstrap menu item is missing.")

    checked_scripts = 0
    for path in (ROOT / "Assets/Scripts").rglob("*.cs"):
        text = path.read_text(encoding="utf-8")
        if not text.strip():
            fail(f"Empty C# script: {path.relative_to(ROOT)}")
        if text.count("{") != text.count("}"):
            fail(f"Unbalanced C# braces: {path.relative_to(ROOT)}")
        checked_scripts += 1

    print(f"Repository sanity checks passed. {checked_scripts} C# scripts inspected.")
    print("Note: Unity compilation, Package Manager import, Play mode, and Android device tests still need Unity.")

if __name__ == "__main__":
    main()
