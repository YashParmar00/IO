#!/usr/bin/env python3
"""Static repository sanity checks; does not replace a Unity compile or device test."""
from __future__ import annotations

import json
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
EXPECTED_EDITOR = "6000.3.26f1"
EXPECTED_CHANGESET = "de8167e7440c"

REQUIRED_FILES = [
    "README.md",
    ".gitignore",
    ".github/workflows/project-sanity.yml",
    "Packages/manifest.json",
    "ProjectSettings/ProjectVersion.txt",
    "Assets/Editor/ProjectBootstrapper.cs",
    "Assets/Scripts/README.md",
    "Assets/Settings/README.md",
    "Assets/Scripts/Config/GameConfig.cs",
    "Assets/Scripts/Core/GameManager.cs",
    "Assets/Scripts/Core/CameraFollow.cs",
    "Assets/Scripts/Input/WormInputReader.cs",
    "Assets/Scripts/Player/PlayerController.cs",
    "Assets/Scripts/Gameplay/ArenaBounds.cs",
    "Assets/Scripts/Gameplay/FoodSpawner.cs",
    "Assets/Scripts/Gameplay/FoodItem.cs",
    "Assets/Scripts/Gameplay/WormController.cs",
    "Assets/Scripts/Gameplay/WormBody.cs",
    "Assets/Scripts/Gameplay/WormCollisionHandler.cs",
    "Assets/Scripts/Gameplay/WormSegment.cs",
    "Assets/Scripts/Gameplay/WormAIController.cs",
    "Assets/Scripts/UI/ScoreManager.cs",
    "Assets/Scripts/UI/ScoreHUD.cs",
    "Assets/Scripts/UI/GameFlowUI.cs",
    "docs/ARCHITECTURE.md",
    "docs/GAMEPLAY.md",
    "docs/RESEARCH.md",
    "docs/MULTIPLAYER_DESIGN.md",
    "docs/ROADMAP.md",
    "docs/TEST_PLAN.md",
    "docs/UNITY_SETUP.md",
    "scripts/validate_project.py",
]

def fail(message: str) -> None:
    print(f"ERROR: {message}", file=sys.stderr)
    raise SystemExit(1)

def require_tokens(path: str, tokens: list[str]) -> None:
    source_path = ROOT / path
    content = source_path.read_text(encoding="utf-8")
    missing = [token for token in tokens if token not in content]
    if missing:
        fail(f"{path} is missing expected architectural token(s): {', '.join(missing)}")

def main() -> None:
    missing = [path for path in REQUIRED_FILES if not (ROOT / path).is_file()]
    if missing:
        fail("Missing required files: " + ", ".join(missing))

    try:
        manifest = json.loads((ROOT / "Packages/manifest.json").read_text(encoding="utf-8"))
    except (OSError, json.JSONDecodeError) as exc:
        fail(f"Packages/manifest.json is not valid JSON: {exc}")

    dependencies = manifest.get("dependencies", {})
    if dependencies.get("com.unity.inputsystem") != "1.20.1":
        fail("Input System should be pinned to the released Unity 6000.3 version 1.20.1.")
    if "com.unity.ugui" not in dependencies:
        fail("uGUI package dependency is required by ScoreHUD and GameFlowUI.")

    version_text = (ROOT / "ProjectSettings/ProjectVersion.txt").read_text(encoding="utf-8")
    if f"m_EditorVersion: {EXPECTED_EDITOR}" not in version_text:
        fail(f"ProjectVersion.txt must pin Editor {EXPECTED_EDITOR}.")
    if EXPECTED_CHANGESET not in version_text:
        fail("ProjectVersion.txt changeset does not match the pinned Editor patch.")

    require_tokens("Assets/Scripts/Player/PlayerController.cs", [
        "WormInputReader", "body.MovePosition", "body.MoveRotation",
        "SetSteeringTarget", "SetPlayerInputEnabled",
    ])
    require_tokens("Assets/Scripts/Input/WormInputReader.cs", [
        "TryGetDesiredDirection", "Touchscreen.current", "Keyboard.current",
        "IsPointerOverGameObject",
    ])
    require_tokens("Assets/Scripts/Gameplay/WormBody.cs", [
        "SeedInitialPath", "UpdateBodySegments", "public void Grow", "HideSegments",
    ])
    require_tokens("Assets/Scripts/Gameplay/WormController.cs", [
        "WormBody", "ArenaBounds", "EatFood", "HandleTriggerContact", "EndRun",
    ])
    require_tokens("Assets/Scripts/Gameplay/WormCollisionHandler.cs", [
        "OnTriggerEnter2D", "owner?.HandleTriggerContact(other)",
    ])
    require_tokens("Assets/Scripts/Gameplay/FoodSpawner.cs", [
        "ObjectPool<FoodItem>", "ArenaBounds", "GetNearestFood", "GetRandomPointInArena",
    ])
    require_tokens("Assets/Scripts/Gameplay/FoodItem.cs", [
        "worm.EatFood", "owner.Release(this)", "consumed",
    ])
    require_tokens("Assets/Editor/ProjectBootstrapper.cs", [
        'MenuItem("Tools/IO/Create-Reset Demo Scene and Prefabs")',
        "LoadOrCreateGameConfig()", "AddComponent<WormInputReader>()",
        "AddComponent<PlayerController>()", "AddComponent<WormBody>()",
        "AddComponent<WormController>()", "AddComponent<WormCollisionHandler>()",
        "AddComponent<ArenaBounds>()", "AddComponent<FoodSpawner>()",
        'SetObjectReference(player.GetComponent<WormInputReader>(), "worldCamera", camera)',
    ])

    generator = (ROOT / "Assets/Editor/ProjectBootstrapper.cs").read_text(encoding="utf-8")
    if 'SetObjectReference(player.GetComponent<PlayerController>(), "worldCamera", camera)' in generator:
        fail("Stale generator reference: worldCamera belongs on WormInputReader, not PlayerController.")

    design = (ROOT / "docs/MULTIPLAYER_DESIGN.md").read_text(encoding="utf-8")
    required_design_tokens = [
        "16 combatant slots", "Free-for-All (FFA)", "Team Deathmatch",
        "Survival Royale", "Server-authoritative dedicated game server",
        "Coins", "Phase 4 — Online FFA vertical slice",
    ]
    missing_design_tokens = [token for token in required_design_tokens if token not in design]
    if missing_design_tokens:
        fail("docs/MULTIPLAYER_DESIGN.md is missing expected decision(s): " + ", ".join(missing_design_tokens))

    readme = (ROOT / "README.md").read_text(encoding="utf-8")
    if "docs/MULTIPLAYER_DESIGN.md" not in readme:
        fail("README.md should link docs/MULTIPLAYER_DESIGN.md.")
    documented_paths = [
        "Assets/Scripts/Config/GameConfig.cs",
        "Assets/Scripts/Input/WormInputReader.cs",
        "Assets/Scripts/Player/PlayerController.cs",
        "Assets/Scripts/Gameplay/WormBody.cs",
        "Assets/Scripts/Gameplay/WormCollisionHandler.cs",
        "Assets/Scripts/UI/ScoreManager.cs",
    ]
    missing_docs = [path for path in documented_paths if path not in readme]
    if missing_docs:
        fail("README does not document these source files: " + ", ".join(missing_docs))

    checked_scripts = 0
    for path in (ROOT / "Assets").rglob("*.cs"):
        source = path.read_text(encoding="utf-8")
        if not source.strip():
            fail(f"Empty C# script: {path.relative_to(ROOT)}")
        if source.count("{") != source.count("}"):
            fail(f"Unbalanced C# braces: {path.relative_to(ROOT)}")

        declared_types = re.findall(r"\b(?:class|struct|enum)\s+([A-Za-z_]\w*)", source)
        if path.stem not in declared_types:
            fail(f"Main type name should match file name: {path.relative_to(ROOT)}")

        checked_scripts += 1

    print(f"Repository sanity checks passed. {checked_scripts} C# scripts inspected.")
    print("Note: this checks structure and cross-file wiring only; Unity compile, import, Play Mode and Android tests still need Unity.")

if __name__ == "__main__":
    main()
