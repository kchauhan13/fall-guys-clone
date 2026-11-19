# SPEC-010 — Round session

- Status: deferred
- Milestone: M5
- Depends on: [SPEC-008](SPEC-008-fall-respawn.md), [SPEC-009](SPEC-009-obstacle-clock.md)

Write the final course layout and the exact qualify count into this file at the start of M5, before building the scene.

## Fantasy

One short race. You see the route, you survive a few bodies-worth of hazards, you cross a finish, and the game tells you if you made the cutoff. Alone, the cutoff is "finish before the timer."

## In scope

A `RoundDefinition` asset: scene name, duration (60–90 s), qualify count. A session state machine: `Countdown`, `Playing`, `Evaluating`. A finish volume that sets `Qualified` on the snapshot. A world-space or screen timer that reads `RoundClock`. One original course using the obstacle driver.

## Out of scope

Multiple rounds in a row, matchmaking, 8 humans, rewards.

## Rules

1. Countdown lasts 3 seconds. Motors ignore commands until `Playing`, except look yaw may still update the camera.
2. On finish overlap, that `PlayerId` is qualified. UI reads the flag.
3. When the clock passes the duration, state becomes `Evaluating`. Qualified runners show a clear success. A runner still on the course shows a clear miss.
4. Falling in a round raises `Fell`. For this first round, fall respawns at the last checkpoint and does not eliminate. Elimination-on-fall is a later round's definition, not a motor change.
5. The Gym scene is unchanged and has no timer.

## Acceptance

- [ ] A full play from countdown to a finish before the timer shows success.
- [ ] Waiting out the timer without finishing shows a miss.
- [ ] The qualify flag is set without the UI script moving the runner.

## Open questions

- Course theme and length in meters. Decide in a blockout pass at M5 and record it here.
- Whether 8-player qualify count is 4. The vision says the first half of the field. For one local runner the timer is the only gate.
