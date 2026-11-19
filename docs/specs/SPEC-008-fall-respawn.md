# SPEC-008 — Fall and respawn

- Status: ready
- Milestone: M3
- Depends on: [SPEC-007](SPEC-007-gym.md)

## Fantasy

Missing a platform costs time, not a loading screen. You are back on the last safe pad quickly, upright, ready to move.

## In scope

Checkpoint volumes and a fall volume. In the Gym, fall respawns. In a round (later), the same fall volume reports `Fell` and the round rules decide between respawn and elimination.

## Out of scope

Lives, a death animation longer than the respawn blend, elimination UI. Those belong to [SPEC-010](SPEC-010-round-session.md).

## Rules

1. Checkpoint volumes are `CourseTrigger` overlaps. Touching one stores that pose for the runner's `PlayerId`.
2. The fall volume raises `Fell`. In the Gym, the app layer teleports the rigidbody to the stored checkpoint, clears velocity and angular velocity, sets `Upright`, and restores rotation locks.
3. Teleport happens in `FixedUpdate` after the motor step, not in `Update`.
4. If no checkpoint was touched, the spawn pose is the checkpoint.
5. The motor does not reference the checkpoint component. The app layer listens for `Fell`.

## Acceptance

- [ ] Running off the Gym floor returns you to the last checkpoint you touched, or spawn if you touched none.
- [ ] After respawn you can move immediately. No extra stumble.
- [ ] Two checkpoints in a row: falling uses the later one.

## Open questions

None.
