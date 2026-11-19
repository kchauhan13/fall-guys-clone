# SPEC-004 — No knock-down

- Status: done
- Milestone: M2
- Depends on: [SPEC-001](SPEC-001-locomotion.md), [ADR-0008](../decisions/ADR-0008-no-knockdown-mario-dive.md)

## Fantasy

The runner is never helpless. Hits shove the body, but the body stays upright and the player keeps the controls. The only time the runner is on the floor is the belly dive, and they get up on their own.

## In scope

The removal of the knock-down: no `Stumbled` state, no `Stumbled` event, no impact threshold, and no ragdoll half of the motor.

## Out of scope

Pushing hazards and blockers (that is [SPEC-009](specs/SPEC-009-obstacle-clock.md)), grab, and the fall volume (that is [SPEC-008](specs/SPEC-008-fall-respawn.md)).

## Rules

1. There is no knock-down state. `MotorState.Stumbled` and the `Stumbled` event are gone.
2. The upright rotation locks stay on during normal play. A collision can push the body, but it cannot tip it over.
3. A landing never knocks the runner down, however fast it is.
4. `GettingUp` is the post-dive recovery only. The dive slide ends in it (see [SPEC-003](SPEC-003-dive.md)).
5. The profile fields `stumbleSpeedThreshold`, `stumbleGetUpDelay`, and `stumbleGetUpSpeedThreshold` are gone. `diveRecoverSpeed` covers the slide speed that ends a dive.
6. Obstacles that used to knock the runner down become pushing or blocking hazards. They move the body with ordinary physics; the motor does not special-case them.
7. Round elimination comes from falling and the timer, never from being knocked out.

## Acceptance

- [ ] Running into a wall, a bar, or any obstacle leaves the runner upright and still steering.
- [ ] Landing from any height leaves the runner upright and still steering.
- [ ] The only way to see the runner on the floor is a dive, and the get-up follows on its own.

## Open questions

None.
