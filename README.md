# IO — Mobile Worm Arena

An original Unity 2D mobile arcade prototype inspired by the broader worm-arena genre. Build a responsive offline single-player loop first; add online systems only when the core game is stable. This repository does not include Worms Zone artwork, code, or branding.

## Toolchain

- **Unity 6.3 LTS, 6000.3.26f1** (recommended baseline; install via Unity Hub).
- **Input System 1.20.1** for touch, pointer, and keyboard steering.
- Android Build Support (Android SDK & NDK Tools and OpenJDK) for Android builds.

Unity released 6000.3.26f1 on 8 October 2026. See [official release notes](https://unity.com/releases/editor/whats-new/6000.3.26f1). iOS builds additionally require macOS and Xcode.

## Included

- Play/pause/resume/game-over state manager.
- Touch/pointer/keyboard steering and Rigidbody2D-based kinematic movement.
- Worm head path tracking, segment following, and growth.
- Capped pooled food, score values, and nearest-food query.
- Simple local AI worms.
- Local best score persistence and score HUD.
- Pause/game-over/restart UI.
- One-click Editor generator for placeholder sprites, prefabs, a demo scene, HUD, bots, and Build Settings.

## Create the demo

1. Install Unity 6.3 LTS 6000.3.26f1 through Unity Hub, then open this repository folder.
2. Let Package Manager resolve dependencies. In Project Settings > Player, set Active Input Handling to **Input System Package (New)** or **Both** if Unity asks.
3. In the Editor menu, choose **Tools > IO > Create-Reset Demo Scene and Prefabs** and confirm. This generates original placeholder circle sprites, BodySegment/Food/PlayerWorm prefabs, Main.unity, HUD, two bot worms, and adds the scene to Build Settings.
4. Open Assets/Scenes/Main.unity and press Play. Use mouse/pointer or WASD/arrow keys to steer. Try the pause and restart buttons.
5. Test touch controls on a physical Android device and check the Console before expanding the project.

The generator has been committed but cannot be run by the GitHub connection itself. Unity still needs to import and execute it locally. The scene/prefabs/build are not claimed as tested until the Editor and device checks pass.

## Layout

- Assets/Scripts/Core — state and camera
- Assets/Scripts/Player — player input and movement
- Assets/Scripts/Gameplay — food, pooling, worm growth, AI, segments
- Assets/Scripts/UI — score, HUD, game-flow controls
- Assets/Editor — one-click starter content generator
- Assets/Scenes, Prefabs, Art — generated starter scene, prefabs, placeholder sprites
- Packages/manifest.json — dependencies
- docs — research, setup, gameplay scope, and roadmap

## Quality rules

Do not add multiplayer, login, ads, analytics, or cloud save until offline play is fun and stable. Never store secrets or sensitive data in PlayerPrefs. See docs/RESEARCH.md for primary Unity references and technical decisions.
