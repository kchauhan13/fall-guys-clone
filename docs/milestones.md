# Milestones

Each milestone ends when its checks pass in play mode. Then update [STATUS.md](STATUS.md). Do not start the next one in the same task unless the human asked for both.

Feel numbers come from [game-feel.md](game-feel.md). "Feels right" means the playtest questions in that file, for the verbs this milestone includes.

## M0 — Architecture

Docs, Cursor rules, and `AGENTS.md`. No gameplay assets.

**Done when:** a new session can tell what to build next by reading `docs/STATUS.md`.

## M1 — Upright movement and jump

Specs: [007 Gym](specs/SPEC-007-gym.md), [001 Locomotion](specs/SPEC-001-locomotion.md), [002 Jump](specs/SPEC-002-jump.md).

**Gate:** from a standstill, reach full speed, stop on a marked 1 m platform, hop a 0.5 m bar, and fall off a lip with coyote time still saving a late jump. Camera is a stiff orbit, good enough to judge camera-relative move.

## M2 — Dive and stumble

Specs: [003](specs/SPEC-003-dive.md), [004](specs/SPEC-004-stumble.md).

**Gate:** dive is obviously faster than running the same straight. A dive off a ledge is scary. A scripted moving bar in the Gym knocks the runner down, and they are upright and steering again in about a second. No grab yet.

## M3 — Camera, fall, one real obstacle

Specs: [006](specs/SPEC-006-camera.md), [008](specs/SPEC-008-fall-respawn.md), [009](specs/SPEC-009-obstacle-clock.md).

**Gate:** the camera stays readable through a dive and a stumble. Falling out of the Gym returns you to the last checkpoint quickly. One clock-driven obstacle (a spinner or a sweeper) hits with a telegraph you can learn in a few tries.

## M4 — Grab

Spec: [005](specs/SPEC-005-grab.md).

**Gate:** you can grab a dummy runner, get dragged, and break the grab on purpose. The dummy can be a second motor with a scripted command source.

## M5 — One round

Spec: [010](specs/SPEC-010-round-session.md).

**Gate:** a 60–90 second course, a finish volume, a visible timer, and a local qualify result for one runner. Still no network. The Gym remains the place tuning happens; the round is not where numbers are invented.

## M6 — Multiplayer spike

Spec: [011](specs/SPEC-011-multiplayer.md).

**Gate:** two game instances in the Gym, each steering its own runner, both bodies colliding. No matchmaking, cosmetics, or account flow. The spike ends by writing which netcode library won and why, as a new ADR. It does not port every round.

## After M6

More courses, session flow for 8 runners, juice, and original art. Each course is its own spec using the obstacle and round contracts. Art production is also the moment to reopen [ADR-0004](decisions/ADR-0004-render-pipeline.md) before a large material library exists.
