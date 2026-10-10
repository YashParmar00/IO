# IO — Mobile Worm Arena

An original Unity 2D mobile arcade game prototype inspired by the broad worm-arena genre. The goal is to build a smooth, offline-first single-player core before adding progression, monetization, or online multiplayer. This project does not include Worms Zone artwork, code, or branding.

## Recommended toolchain

- Unity 6.3 LTS (Editor version family 6000.3), installed through Unity Hub.
- Input System package 1.20.1 for touch, pointer, and keyboard steering.
- Android Build Support (Android SDK & NDK tools and OpenJDK) for Android builds.
- Git with Git LFS only if the team later adds large binary art/audio files.

Use the latest available 6000.3 LTS patch in Unity Hub. Open this repository as a Unity project after cloning; the first Editor import creates generated project files. iOS builds additionally require macOS and Xcode.

## Implemented starter systems

- Game state manager with play, pause, resume, and game-over states.
- Pointer/touch/keyboard steering for a continuously moving worm head.
- Food pooling, food scoring values, and nearest-food lookup for bot steering.
- Worm body path-following and growth.
- Simple AI steering toward nearby food.
- Local best-score persistence using PlayerPrefs.

These scripts are starter systems, not a finished game: the first scene, sprites, prefabs, serialized references, HUD wiring, and platform build settings still need to be authored and verified in Unity Editor.

## Open it

1. Clone this repository and open the project folder in Unity Hub.
2. Install Unity 6.3 LTS (6000.3 family) and Android Build Support if targeting Android.
3. Let Unity resolve Packages and generate Library/ProjectSettings metadata.
4. Follow docs/UNITY_SETUP.md to assemble the first scene and required prefabs.
5. Test in the Editor with mouse or WASD/arrows, then on a physical Android device.

## Repository layout

- Assets/Scripts/Core — run state and game-wide coordination
- Assets/Scripts/Player — player steering
- Assets/Scripts/Gameplay — food, spawning, worm growth, bot steering
- Assets/Scripts/UI — score persistence and HUD
- Assets/Scenes, Prefabs, Art, Audio, Settings — Unity-created content
- Packages/manifest.json — package dependencies
- docs — design scope, setup checklist, and implementation roadmap

## Quality rule

Do not add network multiplayer, login, ads, analytics, or cloud save until the offline loop is fun and stable. Never store secrets or sensitive data in PlayerPrefs. See docs/RESEARCH.md for primary Unity documentation and key decisions.
