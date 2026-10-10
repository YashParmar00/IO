# Manual Test Plan

Run these checks after generating `Main.unity` in the pinned Unity Editor. Record Unity Console errors and the device/editor used.

## Import and setup

- [ ] Unity opens the repository with Editor 6000.3.26f1 and resolves packages.
- [ ] `Tools > IO > Create-Reset Demo Scene and Prefabs` completes without exceptions.
- [ ] Main scene exists, Main Camera is tagged correctly, and Main scene is included in Build Settings.
- [ ] The app default orientation is Portrait.
- [ ] No compile errors or missing script references appear in the Console.

## Gameplay loop

- [ ] Player worm moves continuously when no input is given.
- [ ] Mouse pointer and WASD/arrow keys change heading in Editor Play mode.
- [ ] A physical Android touch changes heading and touching UI does not redirect the worm.
- [ ] Camera follows the player smoothly.
- [ ] Food trigger increases score and adds body segment(s).
- [ ] Eaten food returns to the pool and the food count recovers to the target.
- [ ] Initial segments start spaced behind the head; tail follows turns without severe gaps or jitter.
- [ ] Bots seek food and stop moving while game time is paused.
- [ ] Entering another worm's body/head ends the player's run; an AI worm that hits the player's body is removed.
- [ ] Leaving the configured arena ends the player's run.
- [ ] Pause freezes gameplay; Resume restores it; Restart reloads the scene with time scale at 1.
- [ ] Best score updates in the HUD, persists after game over/backgrounding, and is restored after a fresh launch.

## Mobile checks

- [ ] Verify on at least one physical Android device and more than one portrait screen resolution.
- [ ] Confirm safe areas, buttons, and score labels are readable on tall/narrow screens.
- [ ] Check background/foreground transitions, heat, battery, memory, and sustained frame pacing.
- [ ] Use Unity Profiler on a lower/mid-range device. Tune food and segment caps with measurements, not guesses.
- [ ] Verify release signing, package identifier, icons, and Android build output before publishing.

## Automation coverage

The GitHub Actions job runs repository sanity checks (file presence, manifest JSON, pinned Editor/input versions, and basic script structure). It does **not** compile C# with Unity, generate scene/prefab assets, run Play Mode tests, or build an Android package. Those checks still require Unity Editor and a device/build environment.
