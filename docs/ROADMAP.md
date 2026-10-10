# Implementation Roadmap

## Phase 1 — Source architecture
- [x] Repository layout and Unity Editor baseline pin
- [x] Shared GameConfig ScriptableObject class
- [x] GameManager and camera-follow components
- [x] Dedicated input reader and movement controller
- [x] Dedicated WormBody trail/growth system
- [x] WormController for game rules and ownership
- [x] Dedicated trigger collision handler and tail segment marker
- [x] ArenaBounds shared by food placement and worm exits
- [x] Pooled food, score/growth values, and nearest-food lookup
- [x] Basic interval-based local AI
- [x] Score HUD, pause/game-over/restart UI code
- [x] One-click Editor generator for scene/prefabs/GameConfig/placeholder sprites
- [x] Detailed README, runtime architecture and test plan
- [x] GitHub Actions repository sanity workflow
- [ ] Run generator in Unity Editor and inspect Console/references
- [ ] Verify C# compilation and package import in pinned Editor
- [ ] Verify full food-to-growth loop and tune movement/collisions
- [ ] Test UI, score persistence and restart
- [ ] Test on Android devices and profile

## Phase 2 — Game feel
- [ ] Replace placeholder circles with original art, animation, audio, haptics and feedback
- [ ] Add boost/power-up data and cooldown balancing
- [ ] Improve bot navigation, safe spawn logic and collision fairness
- [ ] Add polished menu, pause/game-over panels and settings
- [ ] Add tests for gameplay rules and spawn/collision edge cases

## Phase 3 — Release readiness
- [ ] Configure app identifier, icons, orientation, signing and Android build
- [ ] Test varied screen sizes, background/foreground transitions, low memory and long sessions
- [ ] Add crash reporting/analytics only after privacy review and consent requirements
- [ ] Evaluate local/cloud progression requirements
- [ ] Consider online multiplayer only with a separate server/network design

## Definition of done for the first playable

A clean clone opens in the pinned Unity Editor, resolves packages, compiles without errors, generates the demo scene without missing references, and plays without runtime exceptions. Player movement feels consistent, food collection drives score and growth, valid collisions/arena exits end the run correctly, pause/restart works, high score persists, and performance is acceptable on a real Android phone.
