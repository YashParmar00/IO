# Implementation Roadmap

This roadmap separates the current offline foundation from future multiplayer work. [MULTIPLAYER_DESIGN.md](MULTIPLAYER_DESIGN.md) is the source of truth for mode rules, server authority, progression and economy. [FEATURE_INVENTORY.md](FEATURE_INVENTORY.md) tracks all feature scope, priority and status; [GAME_FLOWS.md](GAME_FLOWS.md) tracks complete screen journeys, system transitions and failure paths. Review these three planning docs together before each implementation phase.

**Working rule:** complete one phase and its exit gate before starting the next. A design decision does not mean the feature is implemented or production-ready.

## Phase 0 — Design and research baseline (current)

- [x] Record intended match modes and implementation order.
- [x] Record a provisional maximum of 16 combatant slots per match, including bots.
- [x] Define candidate server/network/backend boundaries.
- [x] Define proposed player profile, XP/level rules and a single soft-currency economy.
- [x] Record security principles, technical research, vendor alternatives and open questions.
- [x] Add prioritized feature inventory covering gameplay, screens, multiplayer, progression, safety, settings and live operations.
- [x] Add end-to-end user/system flows for first launch, offline play, matchmaking, gameplay, reconnect, rewards and purchases.
- [x] Record explicit gaps and unresolved decisions rather than treating common genre features as already agreed.
- [ ] Walk through each end-to-end journey and approve required versus deferred features.
- [ ] Resolve feature-level decisions that affect the offline rules before Phase 2 is frozen.
- [ ] Review and approve open design questions before adding multiplayer runtime dependencies.
- [ ] Update the decision log when a proposed rule or provider choice changes.

**Exit gate:** scope, ownership of trusted data and unanswered decisions are clear. No online code is required in this phase.

## Phase 1 — Offline source architecture (mostly complete; Unity validation remains)

- [x] Repository layout and pinned Unity Editor baseline.
- [x] Shared GameConfig ScriptableObject class.
- [x] GameManager and camera-follow components.
- [x] Dedicated input reader and movement controller.
- [x] WormBody trail/growth system and WormController gameplay coordination.
- [x] Trigger collision handler, tail segment marker and shared ArenaBounds.
- [x] Pooled food, score/growth values and nearest-food lookup.
- [x] Basic interval-based local AI.
- [x] Score HUD, pause/game-over/restart UI code.
- [x] Editor generator for scene, prefabs, config and placeholder sprites.
- [x] Documentation, test plan and repository sanity workflow.
- [ ] Open the project in the pinned Unity Editor and run the generator.
- [ ] Verify package import, C# compilation, Console and generated references.
- [ ] Verify food → growth → collision → game-over → restart flow.
- [ ] Verify pause/background behaviour and local best-score persistence.
- [ ] Test on Android hardware and record baseline performance.

**Exit gate:** the offline project compiles and the first-playable checklist passes. The generator and static checks alone do not prove this.

## Phase 2 — Game feel and reusable rules

- [ ] Replace placeholder art as appropriate with original game assets and feedback.
- [ ] Tune steering, arena size, food density, bots and collision fairness on device.
- [ ] Add reproducible tests for food consumption, collision outcomes, score changes and safe spawning.
- [ ] Extract reusable mode-independent rules only where tested behaviour warrants it.
- [ ] Define shared match start/end/result concepts without adding online dependencies prematurely.

**Exit gate:** the offline rules are stable enough to serve as a reference for server simulation.

## Phase 3 — Multiplayer technology spike (proof of concept only)

- [ ] Create a disposable proof-of-concept scene or branch for NGO + Unity Transport.
- [ ] Run a local dedicated server with two clients and validate server ownership.
- [ ] Synchronize a narrow vertical slice (input, movement, collision and one match result).
- [ ] Validate pinned Editor compatibility, headless/server builds, reconnects and authority semantics.
- [ ] Simulate 16 slots and measure CPU, memory, bandwidth and client frame time with target food/body counts.
- [ ] Compare provider allocation workflows, South Asia latency, logs, scaling and realistic monthly cost.
- [ ] Resolve secure server-to-backend authentication and trusted result submission.
- [ ] Choose package versions and vendors based on spike evidence.

**Exit gate:** document the selected stack, alternatives rejected, metrics, cost assumptions and remaining risks. Do not add the full shop/profile system in this spike.

## Phase 4 — Online FFA vertical slice

- [ ] Implement dedicated match lifecycle, join validation, server-authoritative movement and food/body simulation.
- [ ] Implement validated collision outcomes, respawns, score, match timer and result summary.
- [ ] Add the selected lobby/queue/allocation adapter.
- [ ] Add disconnect handling and basic server logs.
- [ ] Test malformed/spammed input, spoofed movement, fake scores, double joins, disconnects and server restarts.
- [ ] Exercise queue cancel/timeout, full/unavailable server, version mismatch, result delay and all error states listed in GAME_FLOWS.md.
- [ ] Validate several clients first, then the full 16-slot target.

**Exit gate:** the FFA match is repeatable and the client cannot choose authoritative movement, score or winner.

## Phase 5 — Persistent profile, XP, Coins and cosmetics

- [ ] Confirm authentication/profile services, data limits, quotas and operating costs.
- [ ] Implement versioned profile data and server-authorized XP/level grants.
- [ ] Implement configurable reward policy and protection against AFK/reward farming.
- [ ] Implement one soft currency, catalogue, owned inventory and equip state.
- [ ] Implement atomic/idempotent purchases with server-side prices and balances.
- [ ] Test retries, duplicate transaction IDs, insufficient balances, already-owned items, reconnects and schema migration.
- [ ] Verify the full shop flow: load → inspect → purchase → authoritative refresh → equip, including ambiguous/retried results.
- [ ] Review privacy, abuse limits and long-term earning/spending balance.

**Exit gate:** profiles, match rewards and shop operations use trusted backend writes and survive retries without double rewards or charges.

## Phase 6 — Team Deathmatch

- [ ] Add server team assignment, team scores and team-specific collision rules.
- [ ] Add respawn logic, timer/score-limit result rules and tie handling.
- [ ] Validate 4v4 before attempting larger teams.
- [ ] Consider 8v8 only if measured server/client budgets pass within the 16-slot cap.

**Exit gate:** team assignment, scoring and winner calculation are server-owned and covered by reproducible tests.

## Phase 7 — Survival Royale

- [ ] Add no-respawn mode and authoritative placement/winner calculation.
- [ ] Add safe-zone phases, damage and warnings using configurable data.
- [ ] Test simultaneous eliminations, disconnects, zone ties, timer expiry and empty/lone-player cases.
- [ ] Tune duration and zone schedule through playtests.

**Exit gate:** every match resolves to a valid authoritative result under the documented edge cases.

## Phase 8 — Production and live-operations readiness

- [ ] Load-test real hosted instances and set resource/cost budgets.
- [ ] Verify South Asia latency, allocation failures, scaling, crash recovery and version compatibility.
- [ ] Add service monitoring, structured logs and recovery/rollback procedures.
- [ ] Review privacy, consent and data retention before analytics or crash reporting.
- [ ] Approve a release checklist and assign operational ownership.
- [ ] Consider ranked queues, parties, missions, chat, monetization or other live-service features only as separate scoped phases.

**Exit gate:** hosting costs, monitoring, support, recovery, security and release responsibilities are accepted.

## First-playable definition of done (Phase 1)

A clean clone opens in the pinned Unity Editor, resolves packages, compiles without errors, generates the demo scene without missing references and plays without runtime exceptions. Movement feels consistent, food collection drives score and growth, valid collisions/arena exits end the run correctly, pause/restart works, best score persists and performance is acceptable on a real Android phone.
