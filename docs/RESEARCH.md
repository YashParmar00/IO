# Research Notes and Technical Decisions

Last reviewed: 10 October 2026.

## Product direction

Build an original, portrait-first, offline worm-arena arcade game. Use the common genre loop—steer continuously, collect food, grow, avoid other worms, survive, and beat the local high score—as a starting point. Add boosts, cosmetics, quests, and online features only after the core loop is fun and measured. Do not copy Worms Zone artwork, UI, sounds, brand assets, or code.

## Engine and packages

- **Unity 6.3 LTS 6000.3.26f1:** official release notes list this patch as released on 8 October 2026. Changeset `de8167e7440c` is pinned in ProjectSettings/ProjectVersion.txt to make the team use one Editor baseline.
- **Input System 1.20.1:** the current 6000.3 package listing documents this input package family. It supports pointer, keyboard, gamepad, and touch input.
- **2D Animation 13.0.6:** version released for Unity 6000.3. It is included for future sprite-skeletal animation/skins; the current prototype does not depend on it.
- **2D Sprite, 2D Tilemap, and uGUI:** core packages are tied to the Editor version. Keep their manifest entries aligned with the selected Editor; if Package Manager flags a core-package version, recreate/update the project through that Editor rather than manually downgrading it.

## Architecture decisions

1. **Single-player before networking.** Bots run locally and are not real multiplayer opponents. Network play needs authoritative rules, latency handling, reconnects, cheating controls, and backend infrastructure; it is a separate milestone.
2. **Transform-driven worm path with kinematic movement.** The worm is an arcade steering object rather than a realistic rigid-body simulation. Head position advances through Rigidbody2D.MovePosition while body transforms sample the head's recent path.
3. **Pool frequently respawned food.** Unity's UnityEngine.Pool API reduces repeated instantiate/destroy overhead and garbage-collection pressure. Keep a capped active count.
4. **Separate responsibilities.** GameManager owns run state; PlayerController handles input and heading; WormController owns tail path/growth and arena exits; FoodSpawner owns food lifecycle; ScoreManager owns score persistence; HUD scripts only display state.
5. **Local best score only for now.** PlayerPrefs is appropriate for simple, non-sensitive local preferences and an offline high score; it is not encrypted and must never store secrets. Cloud save can be evaluated later if cross-device progression is needed.
6. **Generate scene and prefabs inside Unity Editor.** The committed editor utility creates original placeholder sprites, PlayerWorm/BodySegment/Food prefabs, Main.unity, a camera, bots, score HUD, pause/game-over/restart UI, and Build Settings entry. Generated Unity assets are only created when the user runs the menu item.
7. **Android toolchain:** Unity's 6000.3 dependency guide lists Android SDK Build Tools 36.0.0, command-line tools 16, and platform-tools 37.0.1 for 6000.3.26f1+, with NDK r27c and OpenJDK 17. Use the versions managed by Unity Hub to avoid mismatches.
8. **Measure on phones.** Start with capped entities and pooling, then profile on low/mid-range Android hardware before adopting heavier effects or adding lots of bodies.

## Primary references

- [Unity 6000.3.26f1 official release notes](https://unity.com/releases/editor/whats-new/6000.3.26f1)
- [Unity 6.3 LTS manual and version changes](https://docs.unity.com/en-us/engine/6000.3/manual/whats-new)
- [Unity 6.3 released packages](https://docs.unity.com/en-us/engine/6000.3/manual/packages-list/packages-all/pack-safe)
- [Input System package for Unity 6.3](https://docs.unity.com/en-us/engine/6000.3/manual/packages-list/packages-all/pack-safe/com-unity-inputsystem)
- [2D Animation package for Unity 6.3](https://docs.unity.com/en-us/engine/6000.3/manual/packages-list/packages-all/pack-safe/com-unity-2d-animation)
- [Unity object pooling and managed-memory optimization](https://docs.unity.com/en-us/engine/6000.3/manual/scripting/optimization/performance-optimizing-code-managed-memory/reusable-code)
- [ScriptableObject data containers](https://docs.unity3d.com/6000.1/Documentation/Manual/class-ScriptableObject.html)
- [PlayerPrefs API and storage warning](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/PlayerPrefs.html)
- [Android supported SDK, NDK, and JDK versions](https://docs.unity.com/en-us/engine/6000.3/manual/platform-specific/android/getting-started/sdksetup/supported-dependency-versions)
- [Android build and delivery workflow](https://docs.unity.com/en-us/engine/6000.3/manual/platform-specific/android/building-and-delivering)
- [Unity Cloud Save setup](https://docs.unity.com/en-us/cloud-save/get-started)

## Known limitations

- No Unity Editor build/import has been run from this connected environment.
- The editor utility is committed but has not been executed here. Review generated output and fix any Console errors before treating the demo as playable.
- No final art, polished animation, production audio, monetization, backend, or online multiplayer is included.
- Food trigger events require a correctly configured Collider2D setup with at least one Rigidbody2D in each interacting pair, according to Unity's 2D physics rules.
- Current AI only steers toward the nearest available food or wanders; it is not competitive or human-like AI.
- The current score/high score is local to one device/install.
