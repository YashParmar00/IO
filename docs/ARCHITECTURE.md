# Runtime Architecture

## Scene object graph

The Editor generator in `Assets/Editor/ProjectBootstrapper.cs` creates this starter layout inside `Assets/Scenes/Main.unity`:

```text
Main Camera
  CameraFollow -> PlayerWorm
GameRoot
  GameManager
  ScoreManager
FoodSpawner
  pooled Food instances
PlayerWorm
  Rigidbody2D (Kinematic)
  CircleCollider2D (Trigger)
  PlayerController
  WormController
BotWorm_A / BotWorm_B
  same movement + worm systems
  WormAIController
GameCanvas
  ScoreHUD
  GameFlowUI
  score labels, Pause button, state panel
EventSystem
  InputSystemUIInputModule
```

Body segments are separate world-space objects (not children of the head). Food and segment instances are driven by the prefabs assigned to WormController and FoodSpawner.

## Responsibilities and flow

1. **PlayerController** reads touch/pointer or keyboard input, smoothly turns the current heading, and moves the kinematic Rigidbody2D through the physics step.
2. **WormController** records the head trail, samples it for segment positions, adds segments on growth, and ends the player's run on a collision or arena exit.
3. **FoodSpawner** owns a capped Unity ObjectPool of FoodItem objects and fills the arena to its target count.
4. **FoodItem** handles the collectible trigger, asks the worm to eat, then returns itself to the pool.
5. **ScoreManager** increments score, maintains best score in memory, and persists it at game over or when the app is paused/quits. PlayerPrefs contains no secrets.
6. **ScoreHUD** reflects score-change events; it does not own scoring rules.
7. **WormAIController** uses the same movement/worm systems as the player but disables manual input and steers toward the nearest food.
8. **GameManager** owns play/pause/game-over state and freezes scaled gameplay time while paused or over.
9. **CameraFollow** tracks the player with smoothing.

## Physics and input assumptions

- Player and bot heads have kinematic Rigidbody2D components with trigger CircleCollider2D.
- Food and body segment prefabs have trigger colliders. Unity 2D trigger callbacks require at least one collider in a pair to be attached to a Rigidbody2D.
- Player movement uses Unity Input System 1.20.1. Enable **Input System Package (New)** or **Both** in Player Settings if prompted.
- Touches and mouse steering over UI are ignored so HUD controls do not redirect the worm.

## Performance principles

- Food items are pooled instead of repeatedly destroyed and recreated.
- Body following samples the stored path in one pass per frame, rather than scanning the entire path for every segment.
- Food count, pool size, and maximum worm body length are bounded.
- Bot nearest-food selection currently scans active food on each bot Update. If bot/food counts grow, move this query to a short decision interval or spatial index and profile before changing architecture.
- Body segments are instantiated as growth occurs and are not yet pooled; that should be reconsidered only if profiling shows meaningful cost.

## Deliberate exclusions from the MVP

This is a local prototype, not online multiplayer. Bots are not remote players. There is no account system, cloud save, leaderboard backend, ads, purchases, or analytics. Each requires separate technical and privacy decisions after the offline loop is tested.
