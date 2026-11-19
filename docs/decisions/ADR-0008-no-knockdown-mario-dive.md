# ADR-0008 — No knock-down; Mario-style dive that auto-recovers

- Status: accepted
- Date: 2026-10-19
- Amends: [ADR-0002](ADR-0002-hybrid-rigidbody-motor.md)

## Context

Early playtests steered the feel toward a 3D Mario platformer rather than a Fall Guys ragdoll. Being knocked down and flopping — even with a fast get-up — read as a loss of control the player did not want. The dive, however, was liked: it should stay, in the style of the Odyssey dive, and the runner should pick themselves back up on their own.

## Decision

- The runner can **never be knocked down**. There is no stumble from collisions or hard landings. The upright rotation locks stay on except during the dive.
- The **dive** stays: a committed forward burst (`diveSpeed` + `diveUpImpulse`) with a controlled belly-dive pose (an authored forward pitch, not a physics flop), weak steer, and a cooldown.
- When the dive ends and the slide is slow (below `diveRecoverSpeed`), the motor **automatically** plays `GettingUp` (slerp upright over `getUpDuration`) and returns to Upright. No input and no delay.
- Obstacles may still shove the upright body through ordinary physics, but the runner keeps control and stays upright.

`MotorState.Stumbled` and the `Stumbled` event are removed. `GettingUp` now means only the post-dive recovery.

## Consequences

- This amends ADR-0002: the hybrid motor keeps its dive/recover rotation unlock, but the ragdoll/stumble half is dropped. The capsule stays upright in normal play.
- The "bodies are the comedy" pillar is re-aimed: comedy comes from momentum, dives, and physical bumps, not helpless flopping. See [vision.md](../vision.md).
- Profile fields `stumbleSpeedThreshold`, `stumbleGetUpDelay`, and `stumbleGetUpSpeedThreshold` are gone; `diveRecoverSpeed` replaces the last one for the dive slide.
- The M2 Gym sweeper (a knock-down test rig) is removed. M3's clock-driven obstacle ([SPEC-009](../specs/SPEC-009-obstacle-clock.md)) returns as a pushing/blocking hazard, not a knock-down.
- Round elimination comes from falling and the timer, never from being knocked out.
