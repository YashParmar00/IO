# IO Feature Inventory & Scope Register

**Status:** Living product specification — planning only, not a claim that these features are implemented  
**Last reviewed:** 12 October 2026  
**Owner/source of truth:** This document tracks feature scope; [GAME_FLOWS.md](GAME_FLOWS.md) describes user/system flows; [MULTIPLAYER_DESIGN.md](MULTIPLAYER_DESIGN.md) owns mode rules, architecture, progression and economy details; [ROADMAP.md](ROADMAP.md) owns delivery order and exit gates.

## 1. How to read this inventory

Feature status and delivery priority are different:

- **Existing source** — related offline source code exists, but Unity validation is still required.
- **Planned** — approved direction for a later phase; not implemented.
- **Candidate / decision required** — consider and decide before the phase that depends on it.
- **Later** — valuable but not required for the first playable or first online FFA release.
- **Out of initial scope** — explicitly not required for the initial release.

Priority:
- **P0 — foundation / initial release blocker:** must be implemented or explicitly resolved for the named milestone.
- **P1 — next release / retention:** add after the core online loop is stable.
- **P2 — later live-service expansion:** only after product value, safety and operating cost justify it.

A checkmark in the roadmap never means a feature has shipped unless the code, Unity test and acceptance evidence exist. Durations, prices and reward values in the design are tuning proposals, not validated balance.

## 2. Core gameplay and match rules

| ID | Feature | Priority / target phase | Status / scope notes |
|---|---|---|---|
| GAME-01 | Responsive worm steering, touch/pointer input and Editor keyboard input | P0 · Offline foundation | **Existing source; validation required.** Tune on real mobile devices. |
| GAME-02 | Body trail, growth and segment-following | P0 · Offline foundation | **Existing source; validation required.** Keep body/collision visuals independently tunable. |
| GAME-03 | Food spawn, pickup guard, score value and growth reward | P0 · Offline foundation | **Existing source; validation required.** Define the same gameplay semantics for server simulation later. |
| GAME-04 | Collision consequences and arena-exit rules | P0 · Offline foundation | **Existing source; validation required.** The offline prototype currently ends the player run on valid collisions/arena exits. |
| GAME-05 | Local bots and safe spawn placement | P0 · Offline foundation | **Existing basic AI; validation/improvement required.** Improve fairness before using bots to fill online casual matches. |
| GAME-06 | Boost/sprint control, energy/cooldown, visuals and balance | P1 · Game feel | **Candidate.** Decide whether boost ships, how it affects steering/speed, its cost/cooldown, and whether any size/score trade-off exists before implementation. |
| GAME-07 | Temporary power-ups | P2 · Post-core | **Later.** Define each effect, duration, cooldown, stacking and server validation separately. |
| GAME-08 | Food/body-segment drops on elimination | P1 · Multiplayer rule design | **Decision required.** Decide whether a defeated worm leaves collectible drops; define value/lifetime, ownership and anti-farming constraints. Do not infer that body segments automatically become food. |
| GAME-09 | Spawn protection after FFA/TDM respawn | P1 · Multiplayer rule design | **Decision required.** Decide duration and removal conditions; avoid spawn-kill loops without creating exploitable invulnerability. |
| GAME-10 | Safe respawn location, collision-free checks and spawn fairness | P0 · Online FFA | **Planned.** Server selects and validates spawn; client must not choose authoritative spawn position. |
| GAME-11 | Shared match clock, countdown, score/placement and tie-break rules | P0 · Online FFA | **Planned.** Server owns the timer and outcome. Mode-specific tie rules must be specified before competitive/ranked modes. |
| GAME-12 | Pause/resume/restart for offline practice | P0 · Offline foundation | **Existing UI/controller source; validation required.** |
| GAME-13 | Online pause/settings overlay | P0 · Online UX | **Planned.** Opening a local settings overlay must not pause the shared server match or grant a time advantage. |
| GAME-14 | Food, elimination, hit, respawn and match-end feedback | P0 · Core polish | **Partially missing / planned.** Distinct audio/visual/haptic feedback must be configurable and have reduced-motion/sound options where applicable. |

## 3. Modes and real-time match experience

| ID | Feature | Priority / target phase | Status / scope notes |
|---|---|---|---|
| MODE-01 | Solo Practice with local bots | P0 · Offline foundation | Basic source exists; validate locally and preserve offline access. No online Coins/XP from untrusted local results initially. |
| MODE-02 | Online Free-for-All (FFA), max 16 slots total | P0 · First online mode | Planned in [MULTIPLAYER_DESIGN.md](MULTIPLAYER_DESIGN.md). Server-authoritative movement, food, collisions, score, respawns, timer and result. |
| MODE-03 | Team Deathmatch (TDM), initially 4v4 | P1 · After FFA | Planned. 8v8 is conditional on load tests and remains within the 16-slot cap. |
| MODE-04 | Survival Royale, max 16, no respawn, shrinking zone | P1 · After TDM rules | Planned. Server owns zone schedule, eliminations, placements and winner. |
| MODE-05 | Matchmaking queue, searching state and timeout | P0 · Online FFA | Planned. Show mode, elapsed search, retry/cancel options and an actionable error when a match cannot be found. |
| MODE-06 | Match allocation, join validation and start countdown | P0 · Online FFA | Planned. Handle server allocation failure, full match, bad connection details and stale game builds. |
| MODE-07 | In-match scoreboard / current placement | P0 · Online FFA | **Missing from current documented scope; add to the HUD.** Must reflect server-owned score/placement. |
| MODE-08 | Elimination feed / combat event feedback | P1 · Online polish | Planned candidate. Keep it readable on portrait screens; do not let visual effects obscure controls or playfield. |
| MODE-09 | Match results and performance breakdown | P0 · Online FFA | Planned. Show placement, score, food, eliminations, duration and reward breakdown from the trusted match result. |
| MODE-10 | Spectator view after elimination | P1 · Survival Royale | Candidate. Define spectating restrictions and avoid exposing hidden information; not needed for initial FFA if players respawn. |
| MODE-11 | Play Again / return to lobby / queue another mode | P0 · Online UX | Planned. Must recover cleanly from completed, cancelled or failed sessions. |
| MODE-12 | Match rule configuration, durations and reward policy version | P0 · Online server | Planned. Keep tuning data versioned and server-owned; clients must not submit their own rules/reward weights. |

## 4. Screens, navigation and onboarding

| ID | Feature | Priority / target phase | Status / scope notes |
|---|---|---|---|
| UX-01 | Loading, initialization and service unavailable states | P0 · End-to-end flow | Missing as a complete flow. Prevent an endless spinner; offer retry, offline practice where supported, or exit. |
| UX-02 | First-launch onboarding/tutorial | P0 · Before public launch | Missing. Teach steering, food/growth, collision/death, match objective and boost only if boost is implemented. Skippable and replayable. |
| UX-03 | Main menu and mode-selection screen | P0 · Full game flow | Only partial game-flow UI source exists. Design clear primary actions: Play/Practice, Online Modes, Profile, Cosmetics and Settings. |
| UX-04 | Lobby / match-found / countdown screens | P0 · Online FFA | Planned. Show selected mode, player count/status, connection state and start/cancel actions. |
| UX-05 | In-match HUD and pause/settings overlay | P0 · Playability | Existing HUD is primarily score-oriented. Plan score, timer, placement, mode objective, connection state, and mode-specific team/zone indicators. |
| UX-06 | Result → reward → rematch / lobby navigation | P0 · Online FFA | Must be explicitly flow-tested, including a delayed reward response or disconnect. |
| UX-07 | Profile, inventory, cosmetics and shop navigation | P0 · Economy phase | Planned. Every item needs owned/equipped/locked/price states and a safe failure state. |
| UX-08 | Help/tutorial replay, privacy, terms and support links | P0 · Public release readiness | Plan before public release; exact legal content/hosting location needs product review. |
| UX-09 | Empty, loading, retry, disabled, success and error UI states | P0 · Each relevant screen | Missing from the current overall flow specification. Define these alongside the happy path rather than after UI coding. |

## 5. Player identity, profile and social

| ID | Feature | Priority / target phase | Status / scope notes |
|---|---|---|---|
| PLAYER-01 | Solo play without forced account creation | P0 · First playable | Preserve the no-account offline path. |
| PLAYER-02 | Online guest identity / authentication and session validation | P0 · Before public FFA | Provider choice pending. Recommended direction: low-friction guest/anonymous access with a clear future account-linking path; confirm recovery/data-loss trade-offs before implementation. |
| PLAYER-03 | Display name and name validation | P0 · Public online release | Planned. Enforce length/character limits and server-side moderation; do not rely only on client validation. |
| PLAYER-04 | Persistent profile and match statistics | P0 · Progression phase | Planned. Versioned schema; profile is backend-owned, not trusted local PlayerPrefs. |
| PLAYER-05 | Account linking, recovery and cross-device continuity | P1 · Profile expansion | Planned candidate. Avoid promising recovery before the identity provider flow is tested. |
| PLAYER-06 | Account deletion and associated data handling | P0 · If accounts ship publicly | Must be designed to satisfy the applicable store/platform and privacy requirements before release; preserve required transaction/safety records only where lawful and explain the policy. |
| SOCIAL-01 | Friends list, invites and party queue | P2 · Social expansion | Not in initial release. Must define presence, invites, invite expiry, party leader and leader disconnect behaviour. |
| SOCIAL-02 | Private rooms / room codes | P2 · Social expansion | Not in initial release. Needs code expiry, capacity, host/owner permissions and abuse controls. |
| SOCIAL-03 | Text/voice chat | Out of initial scope | Do not add by default. If later proposed, first design moderation, block/mute, reporting, age/privacy and retention requirements. |

## 6. Progression, rewards and economy

The numeric defaults below belong in [MULTIPLAYER_DESIGN.md](MULTIPLAYER_DESIGN.md) and remain provisional until the economy has been simulated/tested.

| ID | Feature | Priority / target phase | Status / scope notes |
|---|---|---|---|
| ECON-01 | XP, level progression and level-up feedback | P0 · Progression phase | Planned; grants must be based on trusted server results. |
| ECON-02 | Coins wallet and match reward summary | P0 · Progression phase | Planned; one soft currency only initially. |
| ECON-03 | Level milestone rewards | P0 · Progression phase | Proposed every-five-level Coins milestones; validate reward curve and prevent duplicate grants. |
| ECON-04 | Cosmetic catalogue / shop | P0 · Economy phase | Planned. Server-owned prices and availability; graceful empty/loading/error states. |
| ECON-05 | Ownership and equip/unequip inventory | P0 · Economy phase | Planned. Reject item IDs not owned and reconcile stale client snapshots. |
| ECON-06 | Atomic/idempotent purchase and reward grant | P0 · Economy phase | Mandatory security property. Retries must not double-charge or double-grant. |
| ECON-07 | Cosmetic-only balance guardrail | P0 · Economy | No speed, damage, vision, steering, hitbox or competitive advantage from cosmetics. |
| ECON-08 | Daily/weekly missions | P1 · Retention | Later. Progress must be verifiable, capped and safe against repeat/replay farming. |
| ECON-09 | Achievements and badges | P1 · Retention | Missing. Define event source, grant uniqueness, profile display and offline-vs-online trust rules. |
| ECON-10 | Global/friends leaderboards and season reset | P1 · Retention/ranked preparation | Missing from the detailed plan. Start with a server-verified lifetime leaderboard; friends/seasonal variants need privacy, abuse and reset rules. |
| ECON-11 | Rotating/limited-time cosmetic shop | P2 · Live content | Later. Requires server-controlled catalogue windows, timezone handling and clear expiry/purchase rules. |
| ECON-12 | Premium currency, real-money purchase and battle pass | Out of initial scope | Do not build now. Revisit separately after product, platform policy, receipt validation and economy goals are approved. |

## 7. Connection resilience, safety and fair play

| ID | Feature | Priority / target phase | Status / scope notes |
|---|---|---|---|
| NET-01 | Connecting/connected/poor connection/lost status | P0 · Online FFA | Planned. Show a compact, readable status without hiding essential gameplay. |
| NET-02 | Disconnect recovery and rejoin grace period | P0 · Online FFA | Policy not final. Define whether the same server can restore a player and what happens when rejoin fails for each mode. |
| NET-03 | Matchmaking cancel, timeout, server full and maintenance errors | P0 · Online UX | Missing as a full set of designed states. Provide retry/back-to-menu choices. |
| NET-04 | Client/server build version compatibility | P0 · Online release | Planned. Reject incompatible builds with a readable update-required state. |
| NET-05 | Input limits, server movement validation and rate limits | P0 · Dedicated server | Planned. Reject malformed/spammed input and prevent client-owned position, speed, score, timer, wallet or result. |
| NET-06 | AFK, disconnect/rejoin and reward farming protection | P0 · Before progression release | Planned. Rules must be transparent and avoid punishing ordinary short connection interruptions unfairly. |
| SAFE-01 | Report player / report suspected cheating | P0 · Public online release | Missing from detailed design. Define categories, evidence policy, rate limits, review workflow and safe acknowledgement UI. |
| SAFE-02 | Server-enforced suspension/ban capability | P0 · Public online release | Missing from detailed design. Require auditable, server-side enforcement and a basic appeal/support policy before public scale. |
| SAFE-03 | Block/mute | P1 or before chat | No chat is in initial scope. If user-generated communication is introduced, block/mute and moderation must be designed before enabling it. |
| SAFE-04 | Player name filtering and abuse handling | P0 · Public online release | Planned requirement paired with display names and reports. |
| SAFE-05 | Privacy, telemetry consent, data retention and account deletion | P0 · Public release readiness | Decide before enabling analytics/accounts/ads; collect only data needed for an explained purpose. |
| SAFE-06 | Server logs, crashes, alerts, support and recovery | P0 · Hosted beta/release | Planned. Keep credentials/tokens out of logs and review any user identifiers in telemetry. |

## 8. Settings, performance, accessibility and presentation

| ID | Feature | Priority / target phase | Status / scope notes |
|---|---|---|---|
| SET-01 | Steering sensitivity and control behaviour | P0 · Mobile polish | Planned. Test touch dead zones and responsiveness across screen sizes. |
| SET-02 | Control layout/handedness | P1 · Accessibility polish | Candidate. Do not assume multiple layouts without testing their usability. |
| SET-03 | Music, SFX and haptics toggles/levels | P0 · Public polish | Planned. Persist user preferences locally; default haptics can be off or conservative. |
| SET-04 | Graphics/performance quality and reduced effects | P1 · Device polish | Planned. Prioritize stable frame time on low/mid-range target phones. |
| SET-05 | Accessibility: readable text, colour contrast and non-colour-only cues | P0 · UI quality | Planned. Ensure team/zone/state cues don't rely solely on colour. |
| SET-06 | Localization/language setting | P2 · Expansion | Later; define supported languages based on audience and QA capacity. |
| SET-07 | App background/foreground, interrupted network and lifecycle recovery | P0 · Mobile/online correctness | Offline pause source exists; online lifecycle behaviour needs a distinct tested policy. |
| SET-08 | Original art, animation, audio and hit/food/death feedback | P0 · Product polish | Some placeholder art only. Use original or appropriately licensed assets; avoid another game's protected branding/assets. |

## 9. Product operations and future content

| ID | Feature | Priority / target phase | Status / scope notes |
|---|---|---|---|
| LIVE-01 | Tunable server-side mode/reward configuration | P0 · Online service | Planned. Validate config version compatibility and safe fallback values. |
| LIVE-02 | Hosting capacity, resource and cost monitoring | P0 · Hosted beta/release | Planned. The 16-slot target must be load-tested, not assumed. |
| LIVE-03 | Match/session identifiers for diagnostics | P0 · Online service | Planned; minimize personal data in logs. |
| LIVE-04 | Rotating arenas/events/seasonal goals | P2 · Live content | Later, after stable gameplay, moderation and operations. |
| LIVE-05 | Ranked skill rating / competitive seasons | P2 · Competitive expansion | Separate from lifetime level; add only after enough players, fair matchmaking and anti-cheat confidence. |
| LIVE-06 | Ads | Out of initial scope | No dependency in current design. Review privacy, store rules and gameplay disruption separately if reconsidered. |

## 10. Cross-screen user journeys that must be tested

These are mandatory journey groups, not just UI screens:

1. First launch → tutorial/skip → solo practice → game over → restart.
2. App returning user → main menu → choose FFA → queue/cancel/timeout → match found → join/countdown → play → result → reward acknowledgement → play again/lobby.
3. Connection lost during each online mode → reconnect attempt → restored session or mode-specific failure/result.
4. Shop load → inspect item → buy → insufficient funds/already owned/transaction retry → authoritative inventory refresh → equip item → show cosmetic in match.
5. Profile refresh → account-link/recovery decision → logout/account deletion route where applicable.
6. Report player → validate/cooldown → acknowledgement → trusted backend stores report for review.
7. App background/foreground during offline play, matchmaking, live match, results and purchase.
8. Server maintenance, incompatible build, allocation failure, empty lobby, full match and backend reward delay.

Detailed transitions and alternate/error paths live in [GAME_FLOWS.md](GAME_FLOWS.md).

## 11. Explicitly deferred unless separately approved

- Real-money purchases, premium currency, loot boxes, battle pass and player trading.
- Text/voice chat, public social feeds and user-generated content.
- Friends/party/private rooms before core online match reliability.
- Ranked mode before population, matchmaking and anti-cheat validation.
- Live events and rotating shop before reward/catalogue configuration and ops are stable.

Deferring a feature is intentional scope control, not forgetting it. Reconsider only through a recorded decision with user value, technical dependencies, moderation/privacy impact and an acceptance test.

## 12. Definition of a complete feature

Every feature must have:
- A unique ID and priority.
- An owner/source-of-truth for its state (client, match server, backend or local-only).
- Happy path plus loading/empty/disabled/error/retry/cancel states where applicable.
- Dependency and data/security assumptions.
- Acceptance criteria and at least one validation path (automated, reproducible manual, device or hosted load test).
- Clear status updated from planned to implemented to tested; documentation alone cannot make it complete.
