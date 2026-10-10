# Gameplay Scope

## Core loop (first playable)

1. Start a run in a bounded arena.
2. Steer a continuously moving worm with touch/pointer position; use WASD or arrow keys in the Editor.
3. Collect food. Each item awards points and adds one or more body segments.
4. Avoid other worms' heads and bodies.
5. End the run on a valid collision and retain the best local score.
6. Restart and try to beat the previous score.

## MVP scope

- Portrait mobile playfield with a camera following the player.
- One player worm with path-following body segments.
- Pooled food items with different score values.
- Local score and best score HUD.
- A small number of basic AI worms.
- Pause/resume/game-over state API.
- Android device testing and a profiling pass.

## After the MVP

- Boost and temporary power-ups with balanced duration/cooldown.
- Skins, unlocks, coins, daily tasks, and menu polish.
- Audio, haptics, hit/food feedback, and accessibility settings.
- More capable bot behavior and spawn safety.
- Analytics, ads, cloud save, account systems, or online multiplayer only after product/privacy requirements and business need are clear.

## Design guardrails

- Keep the steering responsive; tune turn speed and movement speed on real touch devices.
- Avoid unfair spawns near the player or inside a body.
- Separate the visual body shape from collision tuning.
- Keep score and progression rules deterministic and easy to test.
- Use original art, names, sounds, and UI. Genre inspiration is fine; copying another game's protected assets/identity is not.
