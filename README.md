# IO — Mobile Worm Arena

An original Unity 2D portrait mobile arcade game prototype inspired by the broad worm-arena genre. The first milestone is a reliable offline gameplay loop: steer, collect food, grow, avoid other worms, survive, and beat the local best score. This project does not include Worms Zone artwork, code, sounds, or branding.

> **Current status:** the source architecture, starter systems, Editor scene generator, project documentation, and repository sanity workflow are committed. The actual Unity scene, prefab assets, and GameConfig asset are generated locally by the Editor tool. This repository has not yet been imported, compiled, or play-tested by Unity in this environment. Do not treat it as release-ready until the manual validation checklist passes.

## 1. Pinned toolchain

- **Unity Editor:** 6000.3.26f1, pinned in ProjectSettings/ProjectVersion.txt.
- **Input System:** 1.20.1, for pointer/touch and keyboard controls. This package version is listed as released for Unity 6.3.
- **2D Animation:** 13.0.6, reserved for future sprite-skeletal animation and skins; the current runtime does not require it.
- **Android build modules:** install Android Build Support, Android SDK & NDK Tools, and OpenJDK through Unity Hub. Unity documents SDK Build Tools 36.0.0, command-line tools 16, platform-tools 37.0.1, NDK r27c, and OpenJDK 17 for this Editor version family.
- **iOS builds:** require macOS and Xcode in addition to the Unity iOS build module.

References: [Unity Editor 6000.3.26f1 release notes](https://unity.com/releases/editor/whats-new/6000.3.26f1), [Input System package](https://docs.unity.com/en-us/engine/6000.3/manual/packages-list/packages-all/pack-safe/com-unity-inputsystem), [Android dependency matrix](https://docs.unity.com/en-us/engine/6000.3/manual/platform-specific/android/getting-started/sdksetup/supported-dependency-versions).

## 2. Quick start

1. Install Unity 6.3 LTS 6000.3.26f1 from Unity Hub.
2. In Unity Hub, choose **Add → Add project from disk** and select this repository root.
3. Let Unity resolve Packages. If prompted, set Player Settings → Active Input Handling to **Input System Package (New)** or **Both**, then restart the Editor.
4. In Unity Editor, use **Tools → IO → Create-Reset Demo Scene and Prefabs** and confirm.
5. Open Assets/Scenes/Main.unity and press Play. Use the mouse/pointer or WASD/arrow keys to steer.
6. Check the Console, follow docs/TEST_PLAN.md, then test on a physical Android phone.

**Generator warning:** the menu item intentionally replaces Assets/Scenes/Main.unity and the three starter prefabs PlayerWorm, BodySegment, and Food. It also creates placeholder sprite PNGs and Assets/Settings/GameConfig.asset if absent. Do not run it again after making custom edits to those generated files unless you intentionally want to reset them. The generator preserves other Build Settings scene entries and adds/enables Main.unity.

## 3. Folder tree

~~~text
.github/
  workflows/
    project-sanity.yml            # Lightweight repository checks on push/PR
Assets/
  Editor/
    ProjectBootstrapper.cs        # Editor menu tool: creates starter assets and scene
  Art/
    Sprites/                      # Original sprites; demo placeholder PNGs are generated
    Animations/                   # Future animation assets
  Audio/
    SFX/                          # Sound effects
    Music/                        # Music and ambience
  Materials/                      # Optional materials/shaders
  Prefabs/                        # Generated PlayerWorm, BodySegment, Food prefabs
  Resources/                      # Only assets that must be loaded by Resources.Load
  Scenes/
    Main.unity                    # Generated locally by the Editor menu
  Settings/
    GameConfig.asset              # Generated tuning asset; editable in Inspector
  Scripts/
    Config/GameConfig.cs
    Core/GameManager.cs
    Core/CameraFollow.cs
    Input/WormInputReader.cs
    Player/PlayerController.cs
    Gameplay/ArenaBounds.cs
    Gameplay/FoodSpawner.cs
    Gameplay/FoodItem.cs
    Gameplay/WormController.cs
    Gameplay/WormBody.cs
    Gameplay/WormCollisionHandler.cs
    Gameplay/WormSegment.cs
    Gameplay/WormAIController.cs
    UI/ScoreManager.cs
    UI/ScoreHUD.cs
    UI/GameFlowUI.cs
Packages/
  manifest.json                   # Unity package declarations
ProjectSettings/
  ProjectVersion.txt              # Exact Unity Editor version and changeset
  README.md                       # Why the Editor should generate settings
docs/
  ARCHITECTURE.md                 # Responsibilities, dependencies, runtime flow
  GAMEPLAY.md                     # MVP scope and design guardrails
  RESEARCH.md                     # Technical decisions and official references
  ROADMAP.md                      # Phases and remaining work
  TEST_PLAN.md                    # Manual Editor/mobile validation checklist
  UNITY_SETUP.md                  # How to generate and wire the demo scene
scripts/
  validate_project.py             # Static sanity check; does not compile Unity
README.md                         # This project guide and file-by-file map
~~~

Directories containing only a .gitkeep marker are intentional placeholders: Git does not track empty folders. Unity-generated scene/prefab/config assets appear only after the Editor generator is run locally.

## 4. Every source file: what it does and why it exists

### Shared settings and core

| File | Responsibility | Put this kind of logic here |
|---|---|---|
| Assets/Scripts/Config/GameConfig.cs | Defines the shared, Inspector-editable tuning asset. | Movement speed, turning, segment spacing/cap, arena dimensions, food count/score/growth, bot decision interval and frame-rate target. Do not put per-frame gameplay code here. |
| Assets/Scripts/Core/GameManager.cs | Owns run state and the singleton entry point used by gameplay. | Play/pause/resume/game-over state, time-scale freeze/restore, state-change events and target frame rate. New screens should respond to its state rather than maintaining another global game state. |
| Assets/Scripts/Core/CameraFollow.cs | Keeps the camera focused on its target. | Camera smoothing, follow target and camera offset. Camera movement should not be mixed into worm movement or UI scripts. |

### Input and player movement

| File | Responsibility | Put this kind of logic here |
|---|---|---|
| Assets/Scripts/Input/WormInputReader.cs | Reads input and turns it into a desired direction; it never moves the worm. | Touch/mouse-to-world conversion, WASD/arrow support, ignoring pointer presses over UI, and an optional externally supplied target for AI. Add rebindable actions here if a formal Input Actions asset is introduced later. |
| Assets/Scripts/Player/PlayerController.cs | Owns the worm head's heading and physics-step movement. | Turning speed, forward speed, Rigidbody2D.MovePosition/MoveRotation, speed multiplier and calls to WormInputReader. Do not add scoring, food spawning, body trail tracking, or UI to this file. |

### Gameplay systems

| File | Responsibility | Put this kind of logic here |
|---|---|---|
| Assets/Scripts/Gameplay/ArenaBounds.cs | Single source of truth for the rectangular game arena. | Bounds checks and safe random points inside the arena. Change world size in GameConfig or this component's Inspector field; don't hard-code separate bounds into AI and food code. |
| Assets/Scripts/Gameplay/FoodSpawner.cs | Owns food lifetime and placement. | Unity ObjectPool setup, active-food tracking, refill limit, random in-arena positions and nearest-food queries. Keep collectibles themselves lightweight. |
| Assets/Scripts/Gameplay/FoodItem.cs | Represents one collectible food item. | Trigger-based pickup, one-time consumption guard, score value, growth value and simple score-value visuals. It asks the worm to eat, then returns itself to FoodSpawner's pool. |
| Assets/Scripts/Gameplay/WormController.cs | Applies worm gameplay rules and coordinates its components. | Player-versus-bot ownership, scoring on food pickup, arena exits, collision consequences and removing a defeated bot. It delegates body operations to WormBody and trigger detection to WormCollisionHandler. |
| Assets/Scripts/Gameplay/WormBody.cs | Owns trail history and body segments for one worm. | Initial trail seeding, creating segments, growth, storing recent head positions, placing segments along that path, and hiding segments when a bot is removed. It does not read input or award points. |
| Assets/Scripts/Gameplay/WormCollisionHandler.cs | Detects trigger contacts on the head and passes them to WormController. | Classifying the other head or body segment and forwarding the contact. Put collision sensing here; put the outcome (end run or remove bot) in WormController. |
| Assets/Scripts/Gameplay/WormSegment.cs | Marks each tail segment and identifies its owning worm. | Owner reference assigned by WormBody when a segment is created. Avoid movement, score, or AI logic here. |
| Assets/Scripts/Gameplay/WormAIController.cs | Provides the basic offline bot behavior. | Periodically selecting nearby food, setting an external steering target, wandering when needed, and applying bot balance values. It reuses PlayerController/WormInputReader/WormBody/WormController instead of reimplementing a separate worm. |

### User interface

| File | Responsibility | Put this kind of logic here |
|---|---|---|
| Assets/Scripts/UI/ScoreManager.cs | Owns current score and local best score. | Score increments/resets, score-change events, PlayerPrefs best-score persistence at game over or app pause/quit. PlayerPrefs is not encrypted; never store secrets or sensitive data there. |
| Assets/Scripts/UI/ScoreHUD.cs | Displays the scores and subscribes to ScoreManager events. | Updating score and best-score text. Keep business rules out of the HUD. |
| Assets/Scripts/UI/GameFlowUI.cs | Connects Pause, Resume and Restart UI controls to GameManager/SceneManager. | Button wiring and showing the pause/game-over panel based on game state. Add new menu UI here only when it belongs to the game-flow UI. |

### Editor, configuration and verification

| File | Responsibility | Put this kind of logic here |
|---|---|---|
| Assets/Editor/ProjectBootstrapper.cs | Editor-only generator for placeholder art, prefabs and Main scene. | Editor asset creation, component attachment, SerializedObject reference assignment, HUD setup and Build Settings registration. Do not put runtime gameplay logic here. |
| Packages/manifest.json | Declares Unity package dependencies. | Change package versions deliberately, checking that the version is released for the pinned Editor. Core packages such as 2D Sprite and uGUI should match the selected Unity Editor. |
| ProjectSettings/ProjectVersion.txt | Pins the exact Editor baseline. | Update only when the team deliberately upgrades Unity and retests the project. Other Unity settings are generated/managed by the Editor after opening the project. |
| scripts/validate_project.py | Checks repository files, package JSON, version pin, generator entry point and basic C# structure. | Add static repository checks here. It is not a substitute for C# compilation or Play Mode testing. |
| .github/workflows/project-sanity.yml | Runs the Python sanity script on pushes and pull requests. | Fast CI checks that need no Unity installation. Unity compile/build tests are not configured yet. |

### Documentation files

| File | Why it exists |
|---|---|
| docs/ARCHITECTURE.md | Component ownership, scene object graph, input-to-movement and food-to-score flows, physics assumptions and performance choices. |
| docs/GAMEPLAY.md | Defines the first playable loop, MVP scope, later features and design guardrails. |
| docs/RESEARCH.md | Records technical decisions and links to official Unity package, Android toolchain, pooling and persistence documentation. |
| docs/ROADMAP.md | Tracks what is implemented in source versus what still needs Editor/device verification. |
| docs/TEST_PLAN.md | Step-by-step manual tests for import, movement, food, collisions, pause/restart, save persistence and Android behavior. |
| docs/UNITY_SETUP.md | Instructions for opening the project, running the generator, reviewing generated components and validating the scene. |
| Assets/README.md, Assets/Scenes/README.md, Assets/Prefabs/README.md | Short purpose notes for the asset folders, including why Unity should create valid scene/prefab files. |
| ProjectSettings/README.md | Explains that the Editor should generate the full settings folder for the pinned Unity version. |

## 5. Which components belong on which GameObject?

The Editor generator creates these combinations:

| GameObject / prefab | Components | Purpose |
|---|---|---|
| GameRoot | GameManager, ScoreManager | Central run state, score and best score. |
| Main Camera | Camera, AudioListener, CameraFollow | Orthographic camera follows the player head. |
| FoodSpawner | ArenaBounds, FoodSpawner | Defines the world rectangle and owns pooled food. |
| PlayerWorm prefab | SpriteRenderer, kinematic Rigidbody2D, trigger CircleCollider2D, WormInputReader, PlayerController, WormBody, WormController, WormCollisionHandler | Reads player input, moves the head, follows/grows the body, detects collision and applies game rules. |
| BodySegment prefab | SpriteRenderer, trigger CircleCollider2D, WormSegment | One tail segment with an owner reference. WormBody creates instances. |
| Food prefab | SpriteRenderer, trigger CircleCollider2D, FoodItem | One collectible managed by FoodSpawner. |
| BotWorm_A / BotWorm_B | PlayerWorm systems plus WormAIController | Offline AI using the same worm components with manual input disabled and a bot speed multiplier applied. |
| GameCanvas | Canvas, CanvasScaler, GraphicRaycaster, ScoreHUD, GameFlowUI, text labels and buttons | Score/best-score display and pause/game-over/restart controls. |
| EventSystem | EventSystem, InputSystemUIInputModule | Makes Unity UI buttons respond to Input System pointer/touch events. |

The exact serialized references are wired by the Editor generator. If you create these objects manually, follow docs/UNITY_SETUP.md.

## 6. Important logic flows

### Steering

WormInputReader → PlayerController → kinematic Rigidbody2D movement → WormBody records the head path → tail segments sample that path. AI calls PlayerController.SetSteeringTarget; it uses the same movement code as the player.

### Food and growth

FoodSpawner creates/reuses a pooled FoodItem → FoodItem receives a head trigger → WormController awards points (player only) → WormBody adds segments → FoodItem returns to the pool → FoodSpawner refills toward its target count.

### Collision and game over

WormCollisionHandler senses a head trigger → WormController checks the other worm/head segment → player collision freezes the run through GameManager; a defeated AI worm is removed with its segments. Leaving ArenaBounds also ends the player's run or removes an AI bot.

### UI and persistence

ScoreManager emits ScoreChanged → ScoreHUD refreshes labels. GameFlowUI listens to GameManager state changes. Best score is held in memory during play and written to PlayerPrefs when the run ends or the app pauses/quits.

## 7. Coding rules for future work

1. Keep one main Unity component/class per .cs file and make the filename match the component class name. This makes MonoBehaviour scripts easy to attach and find.
2. Put code according to responsibility: input in Input, head movement in Player, tail in WormBody, collision sensing in WormCollisionHandler, rules in WormController, spawning in FoodSpawner, presentation in UI, and shared balance values in GameConfig.
3. Use GameConfig for reusable tuning values instead of duplicating magic numbers across multiple scripts.
4. Expose Inspector fields with [SerializeField] when design-time tuning is needed; avoid making everything public.
5. Avoid scene-wide FindFirstObjectByType calls in frequently executed code. Current fallbacks are setup conveniences; generator-assigned references should be the normal path.
6. Don't add online multiplayer, login, ads, analytics, cloud saving or in-app purchases until the offline core is stable and the product/privacy needs are defined.
7. Replace the placeholder circles with original art once movement/collisions feel correct. Do not copy another game's art or identity.

## 8. What is present versus what Unity must still generate

**Already committed in GitHub:** the C# components, project/package version declarations, Editor generator, README and detailed docs, Python sanity checks, and GitHub Actions workflow.

**Created locally by the Unity Editor menu:** Assets/Scenes/Main.unity, Assets/Prefabs/PlayerWorm.prefab, BodySegment.prefab and Food.prefab, Assets/Settings/GameConfig.asset, and the placeholder sprite PNGs. Unity may also generate .meta files and the rest of ProjectSettings during import. These generated outputs are not yet in GitHub because the Editor tool cannot be executed through the repository connection.

**Still needs verification:** package import, C# compiler, Editor generator execution, actual gameplay, collisions, UI scaling, mobile input, Android build, performance and device testing.

## 9. Checks and troubleshooting

- Start with docs/UNITY_SETUP.md and docs/TEST_PLAN.md.
- Inspect the Unity Console immediately after the first import and after running the generator.
- GitHub Actions only validates repository structure and source sanity. A green action does not prove that Unity compiles or that gameplay works.
- If Unity reports a package version conflict, do not randomly bump core packages. Confirm the selected Editor version and let that Editor resolve its bundled core packages.
- Keep Library, Temp, Obj, Logs and build output out of Git; the provided .gitignore is configured for generated local files.
