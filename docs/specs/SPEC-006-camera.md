# SPEC-006 — Camera

- Status: ready
- Milestone: M3
- Depends on: [SPEC-001](SPEC-001-locomotion.md), [SPEC-003](SPEC-003-dive.md)

## Fantasy

You always know where the runner is facing and where the next hazard is. Hits have a hint of weight. The horizon stays level.

## In scope

Orbit camera, camera-relative motion already provided by `LookYaw`, collision pull-in, dive FOV kick. Cinemachine **2.x** is the rig. A hand-written orbit is allowed only if it still writes the same `LookYaw` the motor already uses.

## Out of scope

Photo mode, split screen, first person, camera shake on every footstep.

## Rules

1. The rig follows the runner chest, not the feet, at `cameraDistance` and `cameraHeight` from `CameraProfile`.
2. Mouse and right-stick look update yaw and a clamped pitch. Yaw is copied into the command each tick. Pitch never goes into the motor.
3. Position smoothing uses `cameraPositionDamp`. Rotation of the rig follows look input directly so steering feels attached to the mouse.
4. A collision along the view line pulls the camera in. It eases back out when the line is clear. It does not pass through walls or into the capsule.
5. On `Dived`, FOV becomes `cameraFov + diveFovKick` and returns over `diveFovDuration`.
6. On `Stumbled`, the rig keeps the last yaw. It does not roll with the body's flop.
7. Qualifying or falling does not blend to a different camera in this spec.

## Acceptance

- [ ] Moving the mouse turns the run direction without a separate rotate key.
- [ ] Standing against a wall does not leave the camera inside the wall or inside the runner.
- [ ] A dive widens FOV briefly and returns.
- [ ] During a stumble the camera stays upright and the body is still in frame.

## Open questions

None.
