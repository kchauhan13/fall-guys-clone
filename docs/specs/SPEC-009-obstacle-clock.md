# SPEC-009 — Obstacle clock

- Status: ready
- Milestone: M3
- Depends on: [SPEC-004](SPEC-004-stumble.md), [contracts](../contracts.md)

## Fantasy

One swinging or sweeping hazard you can learn. Its timing is visible, and the same clock that runs it will later be the server's clock.

## In scope

`RoundClock` driving a single obstacle in the Gym. A serialized curve or a simple ping-pong angle. Kinematic rigidbody so it shoves the runner.

## Out of scope

A full hazard kit, random seeds, networked snapshots of the obstacle. The driver must already be a function of clock time so those can be added without rewriting motion.

## Rules

1. The obstacle's phase is `RoundClock.Time` times a speed value on the obstacle, plus a phase offset. It does not read `Time.time`.
2. The rigidbody is kinematic. The driver sets the next pose in `FixedUpdate` before physics, using the clock.
3. Hitting the runner fast enough causes a stumble through SPEC-004's general rule. No custom knockback script.
4. The mesh color is the hazard blockout color. The collider is a box or capsule child, not the mesh.
5. Motion loops and is readable: you can stand and watch one full cycle, then cross.

## Acceptance

- [ ] Scrubbing or restarting play mode always starts the obstacle at the same phase.
- [ ] The runner can wait out one cycle and cross, and can also be hit and stumble.
- [ ] Searching gameplay scripts in the obstacle folder finds no `Time.time` reads.

## Open questions

Spinner versus sweeper is a Gym choice at build time. Pick one, name the prefab `Spinner` or `Sweeper`, and leave the other for a later course spec.
