# Unity Editor Setup and Validation

## 1. Use the pinned Editor

1. Install Unity 6.3 LTS 6000.3.26f1 with Unity Hub.
2. In Unity Hub choose **Add > Add project from disk** and select the repository root.
3. Let Package Manager resolve Packages. Input System 1.20.1 is pinned for this Editor family. If prompted, set Active Input Handling to **Input System Package (New)** or **Both** and restart.
4. For Android, install Android Build Support, Android SDK & NDK Tools and OpenJDK through Unity Hub. Unity's dependency table for 6000.3.26f1+ lists SDK Build Tools 36.0.0, command-line tools 16, platform-tools 37.0.1, NDK r27c and OpenJDK 17.
5. Wait for the first import to complete and inspect the Console.

## 2. Generate scene and prefabs

In Unity Editor select **Tools > IO > Create-Reset Demo Scene and Prefabs** and confirm. It creates:

- Assets/Settings/GameConfig.asset from GameConfig.cs (preserving an existing tuning asset).
- Original placeholder circles in Assets/Art/Sprites.
- Assets/Prefabs/BodySegment.prefab, Food.prefab and PlayerWorm.prefab.
- Assets/Scenes/Main.unity with GameRoot, FoodSpawner/ArenaBounds, Main Camera, player, two bot worms, HUD, EventSystem.
- A Main scene entry in Build Settings, without dropping the other scene entries.
- Portrait default orientation in Player Settings.

**Destructive warning:** the menu item replaces Main.unity and the three named starter prefabs. It does not replace existing placeholder sprite PNGs or overwrite an existing GameConfig.asset. Run it again only if you intend to reset those generated scene/prefab files.

## 3. Verify component assignments

- **GameRoot:** GameManager references GameConfig; ScoreManager loads local best score.
- **FoodSpawner:** ArenaBounds and FoodSpawner share the generated GameConfig, and FoodSpawner references Food.prefab.
- **PlayerWorm:** Rigidbody2D is Kinematic, head collider is Trigger, and WormInputReader references Main Camera. PlayerController references GameConfig and the reader. WormBody references GameConfig and BodySegment.prefab. WormController references WormBody, ScoreManager and ArenaBounds. WormCollisionHandler references WormController.
- **Bot worms:** same movement/body/controller/collision components plus WormAIController; each AI component references GameConfig, movement, worm rules and FoodSpawner.
- **BodySegment:** SpriteRenderer, trigger CircleCollider2D and WormSegment. Body segments are separate world objects, not children of the head.
- **Food:** SpriteRenderer, trigger CircleCollider2D and FoodItem.
- **HUD:** CanvasScaler uses 1080 x 1920 reference resolution for portrait. ScoreHUD references ScoreManager and its score labels. GameFlowUI references GameManager, status panel and the Pause/Resume/Restart buttons.
- **EventSystem:** EventSystem and InputSystemUIInputModule. The module receives its default input actions when Unity enables it.

If a reference is missing, select the component in the Inspector, reassign the expected field and save the scene/prefab. Prefer updating the generator if a generated assignment is wrong, so the next regeneration stays correct.

## 4. Manual Play Mode checks

Follow docs/TEST_PLAN.md. At minimum:

1. Player moves and turns with mouse/pointer and WASD/arrows.
2. Food is collected once, score updates, growth adds body segments, and food replenishes.
3. Head and tail segments follow through turns without obvious gaps or severe jitter.
4. Bots use food targets, wander when needed, and pause with the game.
5. Head/body collisions and arena exits trigger the intended player game-over or bot-removal behavior.
6. Pause, resume and restart restore timeScale correctly.
7. High score persists after game-over/backgrounding and appears after reload.
8. Unity Console stays clear of compile errors, missing references and runtime exceptions.

## 5. Android device checks

- Build and run on at least one physical phone. Confirm touch steering and ensure tapping UI does not steer the worm.
- Check portrait sizing on different screen ratios; make sure score labels and buttons respect safe areas.
- Check app pause/resume, frame pacing, memory, heat and battery during longer runs.
- Use the Unity Profiler before increasing body length, food count, effects or bot count.
- Configure app identifier, icons, signing and release build settings before publishing.

## 6. What has not been verified automatically

The GitHub connection can write source files, but it cannot execute Unity Editor or Android builds. GitHub Actions checks project structure, JSON, version pins and simple source invariants only. A green CI run is not proof of C# compilation, scene validity, gameplay, or device performance.
