# Unity Editor Setup and Validation

## 1. Open the project

1. Install Unity 6.3 LTS 6000.3.25f1 using Unity Hub.
2. In Unity Hub, choose **Add > Add project from disk** and select this repository root.
3. Let Package Manager resolve dependencies. If Unity asks to enable the new Input System, accept it. Otherwise open Project Settings > Player and set Active Input Handling to **Input System Package (New)** or **Both**, then restart the Editor if prompted.
4. Install Android Build Support, Android SDK & NDK Tools, and OpenJDK in Unity Hub if Android is the first target.

## 2. Generate a demo scene

In Unity Editor, choose **Tools > IO > Create-Reset Demo Scene and Prefabs** and confirm. The Editor script generates placeholder art, prefabs, Main.unity, a camera follow, food spawner, two local bot worms, score HUD, pause/game-over/restart controls, EventSystem, and Build Settings scene registration.

The generator intentionally overwrites its starter scene and the three starter prefabs (BodySegment, Food, PlayerWorm). Do not rerun it after replacing those files with custom work unless you want to reset them. It does not replace existing placeholder sprite files.

## 3. What the generator creates

- PlayerWorm: SpriteRenderer, kinematic Rigidbody2D, CircleCollider2D, PlayerController, and WormController.
- BodySegment: SpriteRenderer, trigger CircleCollider2D, and WormSegment.
- Food: SpriteRenderer, trigger CircleCollider2D, and FoodItem.
- GameRoot: GameManager and ScoreManager.
- FoodSpawner: capped object pool with 90 target food items and a 32 × 20 world area.
- Main Camera: orthographic camera with CameraFollow.
- Canvas: current/best score, pause button, pause/game-over panel, resume/restart buttons, and Input System EventSystem.
- BotWorm_A and BotWorm_B: simple nearest-food-seeking AI.

## 4. Validate before adding features

- Open Assets/Scenes/Main.unity and enter Play mode.
- Confirm mouse steering and WASD/arrows work in the Editor.
- Eat food and verify score changes, the food respawns, and body segments grow.
- Confirm the camera follows, bots move toward food, and the pause/restart buttons work.
- Check that colliding with another worm's head/body ends the player run.
- Confirm the Console has no compile errors or missing references.
- Test on a physical Android phone. Test portrait layout, touch steering, background/foreground transitions, performance, and battery/thermal behavior.
- Do not publish until collisions, game-over/restart flow, UI scaling, and device performance have been verified.

## 5. Important limits

This environment can commit code to GitHub but cannot execute Unity Editor or Android builds. The generator is committed, but the generated scene/prefabs exist only after you run the menu item locally. The prototype is not release-ready; inspect and fix any Editor/runtime issues before continuing.
