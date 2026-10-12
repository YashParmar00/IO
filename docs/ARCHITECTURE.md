# Runtime Architecture

This file describes the **current source architecture**. The Editor generator creates the scene and prefab objects that connect these scripts. Generated Unity assets are not checked in yet; see docs/UNITY_SETUP.md.

## Object graph generated in Main.unity

~~~text
Main Camera
  Camera
  AudioListener
  CameraFollow -> PlayerWorm

GameRoot
  GameManager -> GameConfig
  ScoreManager

FoodSpawner
  ArenaBounds -> GameConfig
  FoodSpawner -> ArenaBounds, GameConfig, Food prefab

PlayerWorm (prefab instance)
  SpriteRenderer
  kinematic Rigidbody2D
  trigger CircleCollider2D
  WormInputReader -> Main Camera
  PlayerController -> GameConfig, WormInputReader
  WormBody -> GameConfig, BodySegment prefab
  WormController -> WormBody, ScoreManager, ArenaBounds
  WormCollisionHandler -> WormController

BotWorm_A / BotWorm_B
  same reusable worm components
  WormAIController -> GameConfig, PlayerController, WormController, FoodSpawner

GameCanvas
  Canvas + CanvasScaler + GraphicRaycaster
  ScoreHUD -> ScoreManager
  GameFlowUI -> GameManager, buttons, status panel

EventSystem
  EventSystem
  InputSystemUIInputModule
~~~

The tool also generates BodySegment and Food prefabs, placeholder circle sprites, the GameConfig asset, Main.unity and its Build Settings entry.

## Component ownership

| Component | Owns | Must not own |
|---|---|---|
| GameConfig | Editable balance values shared via ScriptableObject | Runtime movement, score, or UI state |
| GameManager | Run state, pause/game-over time freeze and state events | Per-worm logic or view layout |
| WormInputReader | Touch/pointer/keyboard and externally supplied AI target | Moving the transform or scoring |
| PlayerController | Head direction, turning and kinematic Rigidbody2D movement | Reading raw devices, tail history, food, UI |
| WormBody | Initial trail, path history, body segment spawn/growth/following | Input, bot decisions, score and collision outcomes |
| WormController | Player/bot role, scoring on pickup, arena exit and collision consequences | Raw input, trail math, trigger sensing |
| WormCollisionHandler | Detecting head collider trigger contacts and forwarding to WormController | Deciding the whole run's outcome |
| WormSegment | Owner reference attached to a tail segment | Segment motion or score |
| ArenaBounds | World rectangle, containment checks, safe random points | Spawning food or deciding game over |
| FoodSpawner | Capped pooled food items, food position and nearest-food queries | Player movement and score rules |
| FoodItem | Pickup guard, score/growth values and food feedback | Pool inventory or UI |
| WormAIController | Periodic bot target choice and wandering | Duplicate movement/body/collision logic |
| ScoreManager | Current/high score, persistence and ScoreChanged event | Rendering text |
| ScoreHUD | Rendering score labels from events | Authoritative score state |
| GameFlowUI | Pause/resume/restart buttons and status panel | Freezing game state independently from GameManager |
| CameraFollow | Camera follow target and smoothing | Player movement |

## Important data flows

### Player steering

WormInputReader reads a desired direction. PlayerController turns toward it in Update and moves the kinematic Rigidbody2D in FixedUpdate. WormBody records the resulting head trail and places its segments along the trail.

Bots call PlayerController.SetSteeringTarget instead of emulating a virtual touch input. The same movement code is reused for the player and bots.

### Food and growth

FoodSpawner owns a Unity ObjectPool of FoodItem components. On a head-trigger pickup, FoodItem uses a consumed guard, then calls WormController.EatFood. WormController awards points only for the player and tells WormBody to grow. FoodItem returns to its owner pool; FoodSpawner refills toward the target count.

### Collision and game state

WormCollisionHandler receives head trigger callbacks and asks WormController to handle the other head or WormSegment. Player collisions and player arena exits end the run via GameManager; bot collisions/exits remove the bot and hide its segments. GameManager emits StateChanged, allowing the HUD to show pause/game-over state. If the app backgrounds while it was playing, GameManager pauses the run and resumes it on foreground return; a manual pause is not auto-resumed.

### Score and persistence

ScoreManager owns score totals and emits ScoreChanged. ScoreHUD renders those values. The best score is held in memory during a run and saved with PlayerPrefs at game-over, app pause, quit, or component disable. PlayerPrefs is not encrypted and must never contain secrets or sensitive data.

## Physics assumptions

- Heads use kinematic Rigidbody2D plus trigger CircleCollider2D; head motion goes through MovePosition/MoveRotation.
- Food and tail segments use trigger CircleCollider2D. Each trigger interaction has a Rigidbody2D on the head side.
- The player must use the new Input System or Both under Player Settings.
- The world arena and food placement both use ArenaBounds to avoid divergent hard-coded playfield sizes.

## Performance decisions

- Food is pooled, and the active count is capped.
- Bot target selection runs at a configurable interval rather than scanning food each rendered frame.
- Body trail history is trimmed to the length needed for the current body; segment position updates walk the path once per frame.
- Worm length, food count and pool capacity are capped.
- Body segments are still instantiated during growth rather than pooled. Only add body pooling if a profiler run indicates a real problem.
- Profile on a real low/mid-range Android device before increasing bot and food counts.

## Future multiplayer direction

The current runtime remains an offline prototype. Future online design and phased gates are documented in [MULTIPLAYER_DESIGN.md](MULTIPLAYER_DESIGN.md) and [ROADMAP.md](ROADMAP.md).

The intended direction is a server-authoritative dedicated match simulation with NGO + Unity Transport as the preferred candidate, subject to a proof-of-concept spike. Mode rules, matchmaking/hosting adapters, profile progression and the soft-currency economy must remain separately owned modules. The mobile client sends input intent; it must not be trusted for position, score, match outcome, XP, wallet balance or inventory ownership.

Do not add networking packages, backend services or new runtime classes as part of this documentation change. First validate the offline game, then measure the multiplayer spike and choose vendors from evidence. The 16-slot cap and match durations are provisional targets until measured.

## Current limitations

- Main.unity, prefabs and GameConfig.asset are generated by the Editor tool; the Unity Editor has not been run in this environment.
- The game has only basic nearest-food AI and placeholder visuals. There is no complete polished start screen, boost system, audio, online play, backend, monetization or cloud save.
- GitHub Actions validates source layout/configuration only. It does not compile C#, open the scene, play the game, or build Android output.
