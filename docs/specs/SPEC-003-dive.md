# SPEC-003 — Dive

- Status: tuning
- Milestone: M2
- Depends on: [SPEC-001](SPEC-001-locomotion.md), [SPEC-002](SPEC-002-jump.md), [ADR-0008](../decisions/ADR-0008-no-knockdown-mario-dive.md)

## Fantasy

A committed belly-flop. It is the fastest move on the course and the easiest way to fly off an edge. When it is over the runner picks themselves up, without being asked.

## In scope

Dive edge, duration, cooldown, upward kick, the authored belly-dive pitch, weak air influence, the slide, and the automatic get-up at the end.

## Out of scope

Dive-jump cancel, grab during dive, FOV kick (that is [SPEC-006](SPEC-006-camera.md)).

## Rules

1. `DivePressed` while `Upright` and `diveCooldown` is clear enters `Diving`.
2. Entering the dive stores horizontal velocity as `diveSpeed` along the flat camera-forward (yaw only), sets vertical velocity to at least `diveUpImpulse` if it was lower, unlocks the pitch and roll locks, and raises `Dived`.
3. The motor drives the body into a belly dive: the pitch eases from the current pitch to `divePitch` over `divePitchTime`, and the angular velocity is zeroed every tick so the body cannot flop under friction. The pitch is an authored pose, not a physics accident.
4. During the dive, stick input may yaw the facing slightly. It does not change speed below `diveSpeed` until the timer ends.
5. After `diveDuration`, a grounded runner slides, bleeding speed with `groundDeceleration`. An airborne runner keeps its momentum until it lands, then slides.
6. When the slide is slower than `diveRecoverSpeed`, the runner enters `GettingUp`. No input is needed and there is no delay.
7. `GettingUp` slerps the body upright over `getUpDuration`, bleeds off the remaining slide speed, and returns to `Upright`, which raises `GotUp`. The rotation locks come back with it.
8. `diveCooldown` starts when the dive state is left, and blocks a new dive for that long.
9. Jump does not interrupt a dive.
10. A dive that starts airborne is allowed. The same impulse applies.
11. Nothing knocks the runner down. A dive never ends in a knock-down, and no collision can put the runner on the floor (ADR-0008).

## Acceptance

- [ ] A timed straight with a dive ends ahead of a timed straight at full run, same start and finish marks in the Gym.
- [ ] Diving toward a short platform can overshoot it if you start too late.
- [ ] Spamming the dive button does not chain dives inside the cooldown.
- [ ] The runner's body pitches into a belly dive and lies flat along the slide.
- [ ] After the slide the runner stands back up on their own and can steer again, with no input and no knock-down.

## Open questions

None. If playtests want dive-cancel, that is a new spec.
