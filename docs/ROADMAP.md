# Implementation Roadmap

## Phase 1 — First playable
- [x] Repository layout and starter documentation
- [x] Game run-state manager
- [x] Pointer/touch/keyboard steering
- [x] Worm path follower and growth API
- [x] Pooled food and nearest-food query
- [x] Local score and best-score persistence
- [x] Simple local AI steering
- [ ] Create/verify Main scene and prefabs in Unity 6.3 Editor
- [ ] Wire HUD and test the full food-to-growth loop
- [ ] Finish and verify collision, game-over, and restart behavior
- [ ] Test on Android devices and profile

## Phase 2 — Game feel
- [ ] Add boost/power-up data and cooldown balancing
- [ ] Add original art, animation, audio, haptics, and feedback
- [ ] Improve bot navigation and safe spawn logic
- [ ] Add settings, pause/game-over panels, and accessibility
- [ ] Add object-spawn telemetry/debug gizmos for development

## Phase 3 — Release readiness
- [ ] Configure app identifier, icons, orientation, signing, and Android build
- [ ] Test varied screen sizes, background/foreground transitions, low memory, and long sessions
- [ ] Add crash reporting/analytics only after privacy review and consent requirements
- [ ] Evaluate local/cloud progression requirements
- [ ] Consider online multiplayer only with a separate server/network design

## Definition of done for the first playable

A fresh clone opens cleanly in the chosen Unity Editor, plays without Console errors, has stable head/body motion, lets the player eat food and grow, updates score/high score, ends the run reliably after a valid collision, restarts correctly, and holds acceptable performance on a real Android phone.
