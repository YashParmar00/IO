# IO Multiplayer, Progression & Economy Design

**Status:** Design specification — planned, not implemented  
**Last reviewed:** 12 October 2026  
**Applies to:** Future online/multiplayer phases of the Unity 2D portrait worm-arena game  
**Decision principle:** Decide the system boundaries now; implement one verified phase at a time.

> This document records the current product and technical proposal so future work can proceed in small, reviewable stages without casually replacing the architecture later. Numbers labelled “target” or “provisional” are starting values for playtesting, not proven balance or production capacity. Do not install networking/backend packages or add multiplayer runtime code merely because this document exists.
>
> For feature coverage and priority, use [FEATURE_INVENTORY.md](FEATURE_INVENTORY.md). For screen transitions, reconnect/error handling, result and shop journeys, use [GAME_FLOWS.md](GAME_FLOWS.md). For phase sequencing, use [ROADMAP.md](ROADMAP.md). These documents complement this technical design; they do not mean the features are implemented.

## 1. Goals and non-goals

### Goals

- Preserve the existing original worm-arena gameplay and Unity GameObject/MonoBehaviour foundation.
- Keep offline practice available while online systems are developed independently.
- Support multiple match modes through shared match lifecycle and mode-specific rules.
- Design for a provisional maximum of 16 combatant slots in an online match, including bots used to fill casual matches.
- Make the dedicated game server authoritative over real-time gameplay; make trusted backend operations authoritative over profile, progression, inventory and currency.
- Keep cosmetic purchases separate from combat advantages.
- Prepare flows, interfaces, data ownership, milestones and acceptance tests before implementation.
- Validate architecture and vendor costs/latency before committing to a hosting provider.

### Non-goals for the first online release

- No more than 16 combatant slots per match.
- No ranked matchmaking at the initial FFA launch.
- No premium currency, pay-to-win items, player trading, loot boxes, battle pass, clans or chat in the initial economy.
- No guaranteed cloud rewards for offline solo play. Solo records remain local until a trusted design is approved.
- No assumption that the Unity client, Relay host, local PlayerPrefs, or a client-submitted match result can be trusted for competitive results or rewards.
- No implementation of all four modes in one release.

## 2. Decision register

| Topic | Current decision | Status / condition |
|---|---|---|
| Product | Original portrait-first mobile worm arena | Agreed direction |
| Maximum online match size | 16 combatant slots total, bots included | Provisional; load-test before release |
| First online mode | Free-for-All (FFA) | Planned first |
| Later modes | Team Deathmatch, then Survival Royale | Planned sequence |
| Network simulation | Server-authoritative dedicated Unity game server | Architectural direction |
| Unity networking library | Netcode for GameObjects (NGO) + Unity Transport | Preferred candidate; validate with a small spike before locking package versions |
| Matchmaking | Keep lobby/queue integration behind an interface; evaluate Unity Multiplayer Services SDK and its hosting integration path | Selected provider/flow not final |
| Dedicated server host | Pilot Edgegap; compare PlayFab Multiplayer Servers and AWS GameLift on real requirements and costs | Not yet selected |
| Persistent player data | Server-controlled profile updates, with Cloud Save + Cloud Code as a candidate | Validate identity, data limits, quotas, pricing and server-to-backend authentication before implementation |
| Economy | One soft currency named Coins | Planned; no premium currency in first economy |
| Cosmetics | Cosmetic-only; no speed/damage/vision/collision advantages | Design guardrail |
| Real-money purchases | Out of initial scope | Revisit only after retention, economy, platform policy and business requirements are clear |
| Ranked play | Separate rating/rank from lifetime level | Later phase |
| Offline rewards | No online XP/Coins from locally claimed solo performance initially | Avoid trusting modifiable local data |

The specific service vendors are replaceable. Game rules and player data must not depend directly on one hosting vendor's SDK. Use narrow adapters so a provider can be changed without rewriting gameplay or the economy.

## 3. Match modes

The durations below are first-balance targets. Tune them using playtests and telemetry; they are not performance promises.

### 3.1 Solo Practice — first local milestone

- Offline arena against local bots.
- Uses the existing movement, body, food, collision and score loop where practical.
- Pause, restart, personal best and settings continue to work without a connection.
- No account required to practise.
- Local personal best may use PlayerPrefs; do not store credentials, secrets or trusted wallet/progression values there.
- Local practice score does not grant online Coins or XP in the first progression release.

**Acceptance gate:** the offline game can be opened, played, paused, restarted and tested without networking or backend services available.

### 3.2 Online Free-for-All (FFA) — first online mode

- Up to 16 total participants in the match, including optional bots. Human matchmaking may start below full capacity.
- Provisional match duration: 4 minutes.
- Every participant competes individually.
- Respawn after elimination; a respawn creates a fresh head/body using validated server-selected spawn positions. The exact spawn-protection duration, if any, is a balancing decision.
- Match score combines validated food/objective points and eliminations. Exact point weights should be configurable and tested.
- End-of-match placement is based on the authoritative final score; deterministic tie-break rules must be documented before ranked play.
- The server owns food placement, collision outcomes, score, elimination, respawn and match timer.
- Disconnections do not pause the whole match. Rejoin grace period and behaviour for a returning player remain an open decision.

**Acceptance gate:** several clients can complete repeated matches against a dedicated server; malicious or malformed inputs cannot directly set position, speed, score, currency or result.

### 3.3 Team Deathmatch (TDM) — after FFA is stable

- First configuration: 4v4.
- Later configuration may support up to 8v8, remaining under the 16-slot cap.
- Provisional match duration: 5 minutes.
- Two teams with server-assigned team membership and server-owned team totals.
- Respawn after elimination.
- Teammate collisions must not award enemy elimination points; enemy collision consequences follow TDM rules.
- Match ends at the timer or a separately configured score limit; choose the score limit through playtesting.
- Leave team balancing, party grouping and disconnect/reconnect policy behind matchmaking rules, rather than hard-coding them into worm movement.

**Acceptance gate:** team assignment, scores, respawns, winner calculation and reconnect cases are validated by automated or reproducible multiplayer tests.

### 3.4 Survival Royale — after TDM rules are reusable

- Up to 16 participants, no mid-round respawns.
- Provisional target duration: 6 minutes.
- Larger starting arena with server-controlled safe-zone phases that progressively shrink the playable area.
- The zone must eventually force encounters; it must not permit indefinite safe draws.
- Server decides zone damage/elimination, final placement and winner.
- The safe-zone phase timings, warning visuals, tie rules and zone damage are configurable balance values.
- Use an original arena/map design; do not copy PUBG maps, branding, art or distinctive assets.

**Acceptance gate:** zone timing is deterministic enough for all clients to see the same phase; the match always resolves to a valid winner/result under tested edge cases.

### Shared match lifecycle

1. Player opens a mode queue and requests a match.
2. Identity and eligibility are checked; matchmaking returns a session/allocation.
3. The client connects to the allocated dedicated server and is authenticated.
4. The server validates the join, assigns a slot/team, creates player objects and sends authoritative initial state.
5. The server starts the countdown and owns the match clock and simulation.
6. The server resolves the result and issues a trusted result record.
7. The backend validates the trusted completion and idempotently grants XP/Coins/stat changes.
8. Clients show results and can return to the lobby or queue again.

A player-facing result screen is presentation only. A result sent by the client is never sufficient to award progression.

## 4. Network and backend architecture

### Responsibilities

**Unity client**
- Touch controls, camera, animation, sound/haptics, HUD, menu, cosmetic preview and result display.
- Sends input intent and user actions, not trusted outcomes.
- Smooths/interpolates remote visuals where appropriate; visual prediction may be considered only after correctness is stable.
- Does not authoritatively choose spawn locations, move speed, food value, damage/collision result, match winner, XP or Coins.

**Authoritative dedicated game server**
- Runs the shared match lifecycle and mode-specific rules.
- Owns movement validation, arena bounds, server-side collision consequences, food spawning/pickups, body growth, eliminations, respawns, team scores, timers and safe-zone progression.
- Validates limits/rates of incoming input to reduce speed hacks, teleporting, malformed packets and spam.
- Emits a trusted match summary only when the match ends; the client cannot forge it.
- Should be headless in production and separately launchable/testable as a server build.

**Persistent game backend**
- Owns authenticated profile, XP, level, balance, inventory, equipped cosmetic IDs, purchase receipts/transaction IDs, match statistics and mission progress.
- Validates and applies reward grants and purchases via server-side code.
- Supports idempotent operations to avoid duplicated rewards after retries or reconnects.
- Keeps secrets and server-to-backend credentials out of the distributed mobile client.

**Matchmaking/session layer**
- Queues players by mode and compatible build/ruleset; later may consider region, latency, skill, party size and input platform.
- Allocates/locates a dedicated server and provides secure connection details.
- Is isolated behind project-owned interfaces. Do not make gameplay scripts depend on provider-specific Lobby, Matchmaker, Relay or hosting classes.

### Technology direction

- **Networking candidate:** Unity Netcode for GameObjects + Unity Transport. The current source is built with GameObjects/MonoBehaviours; validate replication cost, server build compatibility, reconnects, ownership rules and 16-slot load in a small prototype before package adoption.
- **Session services candidate:** Unity Multiplayer Services SDK for sessions, Lobby/Matchmaker/Relay workflows where they fit. The SDK is not itself a universal dedicated-server hosting commitment.
- **Hosting shortlist:** Edgegap pilot; compare PlayFab Multiplayer Servers and AWS GameLift. Compare real deployment/allocation flows, India/South Asia region latency, server CPU/RAM, bandwidth, idle capacity, logs, autoscaling, restart/recovery and estimated monthly cost.
- **Profile/backend candidate:** Unity Authentication + Cloud Save + Cloud Code + Remote Config, subject to an integration spike. The exact trusted machine-to-machine route from the dedicated simulation server to backend reward operations must be resolved before real rewards ship.
- **Do not design around new Unity Economy enrolment:** Unity's official documentation says Economy stopped accepting new project sign-ups on 8 September 2026 and recommends server-authoritative use of Cloud Code, Cloud Save and Remote Config for new projects. Use Unity IAP only if real-money purchases are later approved.

A client-only Relay host is not the preferred authority for competitive results or rewards. If Relay is used for an early networking experiment, label it as a connectivity prototype rather than production match authority.

### Conceptual module boundaries (not files to create yet)

- Networking: bootstrap, transport/session adapter, authentication handshake, input messages and state replication.
- Match: match manager, lifecycle, player spawning, result model and mode interface.
- Modes: FFA rules, TDM rules, Survival Royale rules.
- Gameplay simulation: server movement, food, body, collision, arena and safe-zone rules.
- Progression: profile model, XP/level rules, reward policy and match-result grant.
- Economy: balance operations, price catalogue, inventory and cosmetic equip validation.
- UI: lobby/mode selection, profile, shop, inventory, match HUD and results.
- Observability: structured server logs, disconnect/reason metrics, match duration and anonymized performance metrics subject to privacy review.

Keep the existing offline source working. Extract/reuse pure gameplay rules where tested; do not blindly move every existing MonoBehaviour to the network layer or rewrite the whole game before a measured need appears.

## 5. Progression specification

### Profile fields

The persistent profile will need at least:
- Stable authenticated player ID and display name.
- Current level and total XP.
- Coin balance.
- Owned cosmetic IDs and equipped cosmetic IDs.
- Total completed matches, wins, eliminations and best placement.
- Schema/profile version to support safe future migrations.
- Transaction IDs or equivalent idempotency records for granted rewards and purchases.

Only the server-authorized backend can change trusted fields. The client can cache/display a snapshot, but local cache changes do not grant progress.

### XP and levels

- Start at Level 1 with 0 XP.
- Initial proposed XP requirement to advance from current level L: 100 + 50 × (L − 1). This is a tuning formula, not a permanent economy commitment.
- Award XP for valid match participation/completion, verified performance and victories, with per-mode rules and caps.
- No XP farming by idling, repeatedly joining/leaving or manipulating the client result.
- Keep competitive rank separate from total level; ranked ratings are outside the first launch.

### Milestone rewards

- Proposed reward: 100 Coins at each 5-level milestone.
- Proposed unique cosmetic milestones at selected levels such as 10, 20 and 30.
- Reward tables must be server-side/configurable and versioned; validate the full long-term earning curve before release.
- Daily missions are a later feature. Mission progress must be server-verifiable and capped.

### Solo versus online progress

The first implementation keeps offline personal best local and separates it from trusted online progression. Do not merge local solo rewards into the online wallet unless an explicit anti-cheat/import design is reviewed and accepted.

## 6. Coins, shop and cosmetic economy

### Initial economy

- One soft currency: Coins.
- Proposed initial account balance: 100 Coins.
- Proposed match reward: 10 Coins for a valid completed match, up to 10 additional performance/placement Coins, and 5 Coins for a win.
- Hard cap: 25 Coins per valid match, before separately approved milestone rewards.
- Starting catalogue targets: basic skins around 250 Coins; more elaborate visual cosmetics around 500–1,000 Coins.
- Prices and rewards are initial hypotheses. Test expected matches-to-purchase, time-to-reward, retention and inflation before shipping.

No premium currency or real-money sales are required for this phase. Coins are earned through valid online matches and later approved missions/milestones.

### Cosmetics policy

Allowed: body skins/patterns, trails, colour themes, victory poses and purely visual death effects. Cosmetic items cannot improve speed, steering, collision, vision, food pickup radius or any competitive mechanic.

### Secure shop purchase flow

1. Client requests the published catalogue and current server-owned wallet/inventory snapshot.
2. Client requests purchase of a cosmetic ID; the client does not choose the price or new balance.
3. Backend authenticates the player, loads the canonical catalogue/price and checks ownership and available balance.
4. Backend executes a single atomic/idempotent transaction: validate, deduct Coins, grant owned item and record a unique transaction ID.
5. Backend returns the resulting balance/inventory snapshot; client updates its UI from that response.
6. Duplicate requests, request retries, insufficient funds and already-owned items must not double-charge or duplicate grants.

Never accept price, balance, inventory ownership, XP or level directly from client input. Use server-side configurable prices/rewards, audit logs and transaction idempotency. Real-money purchase integration is a separate platform-policy and receipt-validation design.

## 7. Phased implementation plan and exit gates

**Do not implement all phases together.** Each phase starts only after its previous acceptance gate is met. The repository's existing offline starter and generated Unity assets still require manual Editor validation.

### Phase 0 — Design and research baseline (current)

- [x] Record match modes, provisional cap, architecture boundaries, progression, economy, flows, research references and known open questions.
- [x] Define the 16-slot maximum as provisional and requiring load validation.
- [ ] Review/approve unresolved product decisions before introducing online packages or trusted progression.
- [ ] Keep a design decision log for changes to rules, providers, data ownership or reward math.

**Exit gate:** team accepts scope and open decisions are explicit; no online runtime code is needed for this phase.

### Phase 1 — Validate the offline foundation

- [ ] Open the project in the pinned Unity Editor, run the existing scene/prefab generator, inspect Console and references.
- [ ] Verify C# compilation, food-growth loop, collisions, score, pause/restart and local best score.
- [ ] Test on target Android hardware and profile the offline baseline.
- [ ] Fix or record existing foundation defects before adding network dependencies.

**Exit gate:** offline build compiles and the first playable test plan passes.

### Phase 2 — Game feel and reusable rules

- [ ] Improve original visuals/feedback and bot/spawn fairness as needed.
- [ ] Add reproducible tests for movement limits, collision decisions, score changes, food consumption and spawn validity.
- [ ] Extract mode-independent rules only where tests prove the behaviour; keep UI and input separate.
- [ ] Define deterministic tie behaviour and reusable match start/end events.

**Exit gate:** local rules are sufficiently stable and testable to become the reference behaviour for server simulation.

### Phase 3 — Multiplayer technology spike (no full online game yet)

- [ ] Create a disposable proof-of-concept scene/branch for NGO + Unity Transport.
- [ ] Run a local dedicated server build with two clients; validate server ownership and one synchronized movement/collision path.
- [ ] Test server-side build/export compatibility with the pinned Unity version.
- [ ] Benchmark 16 simulated player slots, food and body-segment updates; measure server CPU, memory, network bandwidth and client frame time.
- [ ] Compare provider deployment flows/costs and India/South Asia latency. Resolve secure game-server-to-backend authentication before granting real rewards.
- [ ] Decide concrete versions and providers only after the spike evidence is reviewed.

**Exit gate:** a documented technology choice, measured baseline, provider cost comparison and known limitations. No shop/progression features yet.

### Phase 4 — Online FFA vertical slice

- [ ] Implement dedicated server lifecycle, validated input, food/body replication and authoritative collision/scoring.
- [ ] Add mode queue/session allocation, join validation, spawn, match timer and result summary.
- [ ] Add disconnect handling and basic server logs.
- [ ] Test spoofed movement/score, invalid messages, double joins, early disconnects, server restart and 2–16 participant scenarios.

**Exit gate:** repeatable 16-slot test passes; no client-authoritative score/result path remains.

### Phase 5 — Persistent profile, progression and Coins

- [ ] Choose and configure identity/profile services after the spike.
- [ ] Implement profile schema/versioning, XP/level calculation, reward table and server-authorized result grants.
- [ ] Add balance, catalogue, owned inventory and equip APIs; secure purchase transaction and idempotency.
- [ ] Test retry/double-submit, insufficient balance, item already owned, reconnect, stale client cache and schema migration.
- [ ] Review reward farming, AFK behaviour, rate limits, data quotas, costs and privacy before public use.

**Exit gate:** match results and purchase transactions are trusted, idempotent and covered by tests.

### Phase 6 — TDM

- [ ] Add server team assignment, team scoring, respawn and enemy/teammate collision rules.
- [ ] Add timer/score-limit tie and disconnect behaviour.
- [ ] Validate 4v4 first; consider 8v8 only after capacity tests.

**Exit gate:** team results are reproducible and cannot be set by clients.

### Phase 7 — Survival Royale

- [ ] Add no-respawn mode, safe-zone phases/damage, placement and final-winner calculation.
- [ ] Test edge cases: simultaneous eliminations, last-player disconnect, timer expiry, zone ties and empty servers.
- [ ] Tune duration and zone schedule through playtests.

**Exit gate:** every completed match records one valid result, including tested edge cases.

### Phase 8 — Production and live operations readiness

- [ ] Load-test real server instances and set capacity alerts and cost budgets.
- [ ] Verify South Asia region latency, autoscaling/allocation failures, crash recovery, abuse protection and version compatibility.
- [ ] Review analytics/privacy/consent and retention policies before collecting telemetry.
- [ ] Add ranked matchmaking, parties, missions, real-money purchases or other live-service features only as separately approved phases.

**Exit gate:** release checklist, rollback/recovery procedure, support/monitoring ownership and operating budget are approved.

## 8. Test and release principles

- Test each mode independently and at boundary values; do not consider a feature complete merely because it works in a single-editor Play Mode session.
- Simulate latency, jitter, packet loss, disconnects, duplicate requests, reconnects, malicious input and server restart.
- Verify client UI always reflects server-owned score/wallet/inventory after an authoritative update.
- Verify game rules and reward tables are tunable without editing unrelated movement/UI code.
- Benchmark worst-case 16-player load with expected food, body length, effects and network updates; reduce work or revise the target if the budget fails.
- Keep provider SDK integrations behind adapters. Keep server secrets out of client builds.
- Record the test environment, build/version, result, bottleneck and acceptance status for each phase.
- A passing static repository check is not proof that Unity compiles, a dedicated server runs, the 16-slot target is met, or online play is production-ready.

## 9. Open decisions — resolve when relevant, not all today

These decisions are intentionally left open until the appropriate implementation phase:
1. Sign-in methods, guest account upgrade and account recovery.
2. Region selection, ping threshold, matchmaking fill timeout and whether casual matches use bots to fill slots.
3. Party/invite flow, cross-platform support and privacy/chat requirements.
4. Rejoin grace period, disconnect forfeits and AFK thresholds.
5. Exact FFA score weights, TDM score limit and respawn-protection duration.
6. Survival Royale zone schedule/damage and placement tie rules.
7. Server-to-backend service authentication and result signing/validation mechanism.
8. Hosting vendor, region, server size, autoscaling and monthly budget.
9. Cloud profile schema, backup/export/retention and data migration.
10. Reward curve, catalogue prices, mission cadence and whether monetization is ever needed.
11. Crash/analytics provider and applicable privacy/consent requirements.

Resolve an item when its phase depends on it; do not invent a vendor or permanently tune an economy before measurements exist.

## 10. Research references

Review official or provider documentation again when selecting package versions or deploying a service, because availability and pricing can change.

- Unity Multiplayer Services SDK (sessions, Lobby, Matchmaker and Relay): https://docs.unity.com/en-us/mps-sdk/index
- Unity Netcode overview and options: https://docs.unity.com/en-us/multiplayer/netcode/netcode
- Unity Cloud Code server authority: https://docs.unity.com/en-us/cloud-code/server-authority
- Cloud Code with Cloud Save tutorial: https://docs.unity.com/en-us/cloud-save/tutorials/cloud-code
- Cloud Save player data protection: https://docs.unity.com/en-us/cloud-save/concepts/player-data
- Unity Economy service status and recommended approach for new projects: https://docs.unity.com/en-us/economy
- Unity Multiplay Hosting lifecycle note: https://docs.unity.com/ugs/en-us/manual/game-server-hosting/manual/concepts/authentication-service-accounts
- Edgegap Unity dedicated server/NGO guide: https://docs.edgegap.com/docs/sample-projects/unity-netcodes/unity-netcode-on-edgegap
- Edgegap integration with Unity Matchmaker: https://edgegap.com/integration/how-to-deploy-dedicated-servers-from-unity-gaming-services-ugs-matchmaker-with-edgegap
- Microsoft PlayFab Multiplayer Servers: https://learn.microsoft.com/en-ca/gaming/playfab/multiplayer/servers/
- AWS GameLift Servers: https://docs.aws.amazon.com/gameliftservers/

### Time-sensitive research notes (checked 12 October 2026)

- Unity Economy documentation states new project sign-ups stopped on **8 September 2026**. It recommends Cloud Code, Cloud Save and Remote Config for new server-authoritative economies, with Unity IAP for real-money purchases.
- Unity's Editor upgrade documentation states direct Unity Multiplay Hosting support ended after **31 March 2026**. Do not select the old direct Unity Multiplay Hosting integration for this new project without re-verifying the replacement/continuity path.
- Edgegap documents a path to integrate Unity Matchmaker with Edgegap dedicated server allocation; this is a candidate to validate, not a decision that the production host has already been selected.
