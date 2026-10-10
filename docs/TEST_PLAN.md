# Manual Test Plan

Run these checks after generating Main.unity in the pinned Unity Editor. Record the Editor version, device, Console errors and any reproduction steps.

## Import and setup

- [ ] Unity opens the repository with Editor 6000.3.26f1 and resolves Packages.
- [ ] Tools > IO > Create-Reset Demo Scene and Prefabs completes without exceptions.
- [ ] GameConfig.asset is created if absent, and existing tuning changes are preserved if it already exists.
- [ ] Main scene exists, Main Camera is tagged correctly, and Main scene is included in Build Settings without deleting other scenes.
- [ ] Portrait orientation is set.
- [ ] No compile errors or missing script references appear in the Console.
- [ ] PlayerWorm Inspector shows Rigidbody2D, WormInputReader, PlayerController, WormBody, WormController and WormCollisionHandler.
- [ ] WormInputReader has Main Camera; PlayerController has GameConfig and WormInputReader; WormBody has GameConfig and BodySegment; WormController has WormBody, ScoreManager and ArenaBounds.
- [ ] FoodSpawner has GameConfig, ArenaBounds and Food prefab references; bots reference their movement/controller/spawner.

## Gameplay loop

- [ ] Player worm moves continuously when no input is given.
- [ ] Mouse pointer and WASD/arrow keys change heading in Editor Play mode.
- [ ] A physical Android touch changes heading and touching UI does not redirect the worm.
- [ ] Camera follows the player smoothly.
- [ ] Food trigger increases score once and adds body segment(s).
- [ ] Eaten food returns to the pool and the active food count recovers toward the configured target.
- [ ] Food visual size/tint reflects its score value.
- [ ] Initial segments start spaced behind the head; tail follows turns without severe gaps or jitter.
- [ ] Bots periodically seek food, wander if no food is available, and stop selecting targets while game time is paused.
- [ ] Player collisions with a different worm head/body cause game over; an AI worm that collides with another worm or leaves the arena is removed.
- [ ] Leaving the configured arena ends the player's run.
- [ ] Pause freezes gameplay; Resume restores it; Restart reloads the scene with timeScale at 1.
- [ ] Backgrounding the app while playing pauses the run; returning to the foreground resumes only if the pause was automatic.
- [ ] Best score updates in the HUD, persists after game over/backgrounding, and is restored after a fresh launch.

## Mobile checks

- [ ] Verify on at least one physical Android device and more than one portrait screen resolution.
- [ ] Confirm safe areas, buttons, and score labels are readable on tall/narrow screens.
- [ ] Check background/foreground transitions, heat, battery, memory, and sustained frame pacing.
- [ ] Use Unity Profiler on a lower/mid-range device. Tune food and segment caps with measurements, not guesses.
- [ ] Verify release signing, package identifier, icons, and Android build output before publishing.

## Automation coverage

The GitHub Actions job checks required files, manifest JSON, the pinned Editor/input versions, cross-component architecture markers, README coverage and simple C# brace balance. It does **not** compile C# with Unity, generate scene/prefab assets, run Play Mode tests, or build an Android package. Those checks still require Unity Editor and a device/build environment.
