# Unity Editor Setup Checklist

This repository contains C# systems and package declarations. Unity scenes and prefabs have not yet been generated or tested, so do this in the Editor before expecting Play mode to work.

## 1. Open the project

1. Install Unity 6.3 LTS, Editor version family 6000.3, using Unity Hub.
2. Clone this repo and choose **Add > Add project from disk** in Unity Hub, pointing at the repository root.
3. Let the Package Manager resolve dependencies. If Unity asks to enable the new Input System, accept it. Otherwise open Project Settings > Player and set Active Input Handling to **Input System Package (New)** or **Both**, then restart the Editor if prompted.
4. Install Android Build Support, Android SDK & NDK Tools, and OpenJDK in Unity Hub if Android is the first target.

## 2. Create the first scene

1. Create and save a scene as Assets/Scenes/Main.unity.
2. Add an empty GameObject named GameRoot. Add GameManager and ScoreManager to it.
3. Add another empty GameObject named FoodSpawner. Add FoodSpawner and assign the food prefab after creating it. Leave its center at (0,0) initially and set an arena size such as 32 by 20.
4. Set the project default orientation to Portrait in Player Settings. Add Main.unity to the active Build Profile's scene list.

## 3. Create the food prefab

1. Create a Sprite GameObject with a simple original circular sprite. Name it Food.
2. Add CircleCollider2D and enable Is Trigger.
3. Add FoodItem.
4. Drag it into Assets/Prefabs to create a prefab.
5. Assign this prefab to FoodSpawner's Food Prefab field.

At least one of the two objects in a 2D trigger interaction needs a Rigidbody2D. The player head setup below supplies it.

## 4. Create the player worm

1. Create a GameObject named PlayerWorm with a SpriteRenderer using an original placeholder sprite.
2. Add Rigidbody2D, set Body Type to Kinematic and Gravity Scale to 0.
3. Add CircleCollider2D. Keep its trigger setting consistent with the collision behavior you want.
4. Add PlayerController and WormController.
5. Make a separate BodySegment prefab with SpriteRenderer, CircleCollider2D (Is Trigger enabled), and WormSegment. Use a smaller visual scale than the head, then assign it to WormController's Segment Prefab field.
6. Assign the scene ScoreManager to WormController's Score Manager field. The script attempts to find one automatically if left empty, but explicit references are easier to debug.
7. On Main Camera, set Projection to Orthographic, add CameraFollow, assign PlayerWorm as Target, and set Offset to (0,0,-10). Assign this camera to PlayerController's World Camera.

Body segments are spawned as separate world-space objects; do not make the segment prefab a child of the head.

## 5. Add an AI worm

1. Duplicate the player worm and place it away from the starting position.
2. Set its WormController Starting Segments to a small value and assign the same segment prefab.
3. Add WormAIController. Assign its PlayerController, WormController, and FoodSpawner fields, or let it attempt scene lookup.
4. Repeat for only a few bots initially. Tune speed before increasing the count.

## 6. Add the HUD

1. Create a Canvas set to Screen Space - Overlay.
2. Add two Unity UI Text elements, one for current score and one for best score.
3. Add ScoreHUD to a UI GameObject and assign both Text fields plus ScoreManager.
4. Add a Canvas Scaler set to Scale With Screen Size, using a reference resolution of 1080 × 1920 for the first portrait layout.

## 7. Validate before adding features

- Enter Play mode and confirm mouse steering and WASD/arrows.
- Confirm the camera follows the head.
- Confirm food disappears when eaten, returns to the pool, respawns, adds score, and grows the worm.
- Confirm tail segments follow without large gaps or jitter.
- Test pause/resume and time-scale restoration.
- Test on a physical Android phone; inspect Console and Profiler, then tune arena food count and bot count.
- Do not publish a build until collisions, game-over/restart flow, UI scaling, and device performance have been tested.

## Input note

The current PlayerController uses the Input System directly for pointer/touch and keyboard input rather than an Input Actions asset. Add an Input Actions asset only if rebinding, gamepad support, or more complex menus become necessary.
