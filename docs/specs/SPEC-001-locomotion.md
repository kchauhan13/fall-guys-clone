# SPEC-001 — Locomotion

- Status: done
- Milestone: M1
- Depends on: [SPEC-007](SPEC-007-gym.md), [contracts](../contracts.md)

## Fantasy

The runner takes a beat to reach speed, slides a little when the camera turns hard, and stops on purpose.

## In scope

Upright ground movement from `PlayerCommand.Move` and `LookYaw`. Slopes up to `slopeLimit`. Air steering is implemented here at `airControlMultiplier` so the jump spec can assume it.

## Out of scope

Jump impulse, dive, stumble, grab, animated legs, gamepad-specific curves.

## Rules

1. Wish direction is `Move.y` forward and `Move.x` right, rotated by `LookYaw` around world up. The stick is clamped to length 1 before this.
2. Wish speed is stick magnitude times `maxGroundSpeed`.
3. While `MotorState.Upright` and grounded, horizontal velocity moves toward the wish velocity at `groundAcceleration` when speeding up or turning, and at `groundDeceleration` when the stick is shorter than the current speed.
4. `turnResponsiveness` limits how fast the facing yaw catches `LookYaw` plus the stick's facing. The velocity still slides, so the body does not snap to the new direction in one tick.
5. While airborne and upright, horizontal acceleration is `groundAcceleration * airControlMultiplier`. Vertical velocity is untouched by this spec.
6. Rotation constraints freeze X and Z while upright. Yaw is free.
7. Grounded means a downward cast, distance a few centimeters, hits `Ground` or `Obstacle`, and the normal is within `slopeLimit` of up.
8. Steeper contacts do not count as ground and do not receive wish velocity along the wall.
9. The motor writes horizontal velocity directly (velocity change). Gravity remains the project gravity on Y.
10. A zero stick grounded does not add acceleration; deceleration brings horizontal speed to zero.

## Acceptance

In the Gym, `MovementProfile` set to the Start column:

- [ ] Holding forward reaches nearly `maxGroundSpeed` and does not exceed it on flat ground.
- [ ] Releasing the stick stops the runner in well under a second.
- [ ] A hard 90° turn at full speed travels outward before the new direction wins.
- [ ] A 45° slope marked in the Gym is walkable. A 70° wall is not a floor.
- [ ] The console shows no input reads inside `Tumble.Simulation`.

## Open questions

None.
