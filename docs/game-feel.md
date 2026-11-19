# Game feel

This is the only place feel numbers live. [contracts.md](contracts.md) names the fields. When a playtest changes a number, edit the **Start** column here and append a row to [tuning-log.md](tuning-log.md).

These starts are hypotheses for the Gym. They are not sacred. The adjectives below are sacred until the vision changes.

## Adjectives

Weighty. Loose. Committed. Recoverable. Readable.

A good second of motion: the runner eases up to speed, drifts through a turn, hops lower than a cartoon mascot platformer, and stops a little faster than it starts so landings still obey the player.

A bad second of motion: instant full speed, ice with no control, a floaty hang, or a dive that leaves the runner sliding on with no way back to the controls.

## Verbs

### Move

Stick deflection sets a wish speed from zero up to max ground speed. Acceleration is constant while speeding up, so the first steps feel heavy. Deceleration is stronger than acceleration, so releasing the stick stops the runner before a full second passes. Turning lags behind the stick: sharp flicks slide outward, then catch. Slopes up to the slope limit are walkable. Steeper contacts are not sticky walls: the runner keeps control and stays upright.

Air control uses the same wish direction at `airControlMultiplier` of ground acceleration. You can steer a jump. You cannot stop it.

### Jump

A variable-height hop in the style of a classic platformer: tap for a low hop, hold to float up to a capped maximum. The press sets the initial `jumpSpeed`; while rising, holding the button uses the lighter `riseGravityMultiplier`, and releasing early switches to the heavier `lowJumpGravityMultiplier` which cuts the arc short. Holding for about the whole ascent reaches the maximum; you cannot exceed it. Falling always uses the largest `fallGravityMultiplier`. Releasing latches the cut for the rest of that rise, so a mid-air re-press cannot restore the float. Coyote time and a jump buffer forgive lips and early presses. One jump until grounded. The buffer, coyote, and cut all live in the motor so a later server can own them.

### Dive

The fastest way across a gap and the easiest way to overshoot a landing. A dive impulse replaces horizontal velocity with `diveSpeed` along the flat look direction, plus a small upward kick so it clears a lip. The body pitches forward into a belly dive over `divePitchTime`, up to `divePitch` degrees: an authored pose the motor drives, not a physics flop. Control during the dive is almost none: stick input can nudge the facing, it cannot cancel the speed. When the duration ends, a grounded runner slides, bleeding speed with ground deceleration. Airborne divers keep their momentum until they land, then slide. When the slide is slow (below `diveRecoverSpeed`), the runner **gets back up by themselves**: the motor slerps the body upright over `getUpDuration` and hands control back. No input, no delay, no knock-down.

Dive is an edge. It cannot restart until `diveCooldown` has passed after the dive ends. Jump does not cancel a dive in the prototype.

### Getting up

There is no knock-down (ADR-0008). The only way to reach `GettingUp` is the end of a dive. The motor slerps the body upright over `getUpDuration`, damps the angular velocity, bleeds off the remaining slide speed, then locks pitch and roll and returns to `Upright`. The runner never loses control to a hit: obstacles shove the upright body, and the body stays upright.

### Grab

A held reach, about a body-width in front of the chest. On contact with another runner or a grabbable prop, a joint holds until the button releases, the distance exceeds `grabBreakDistance`, or either body is mid-dive. Grab is specified in [SPEC-005](specs/SPEC-005-grab.md) and is not part of the first Gym sessions.

## Camera

Third person, behind the runner, orbiting with the mouse or right stick. Movement is relative to camera yaw, and that yaw is written into the command so simulation does not need a camera object.

The camera sits `cameraDistance` back and `cameraHeight` up, with a slight look-ahead along horizontal velocity. Position damping is short, so hits have a beat of weight and then catch up. Dive adds `diveFovKick` for `diveFovDuration`, then returns. The rig pulls closer when geometry blocks the view. It never clips inside the runner.

M1 may use a stiff orbit with no FOV kick. The adjectives still apply: the runner stays readable in frame, and the horizon does not roll.

## Course readability

- Ground, hazard, and finish use three distinct blockout colors, documented in [content-pipeline.md](content-pipeline.md).
- A hazard telegraphs its path with its shape and a repeating motion. No instant-on hitboxes.
- The safe line is visible from the round start camera for at least the first obstacle.
- Failures in the Gym respawn at the last checkpoint in under a second, with no results screen.

## Juice hooks

Presentation listens. It does not decide gameplay. The motor raises these moments: started moving, jumped, dived, landed, got up, grabbed, grab broke, qualified, fell.

M1 needs no VFX. The hooks exist in the API so later juice does not reach into the motor and write velocity.

## Starting profile

Units are seconds, meters, and meters per second unless noted. Gravity in Project Settings stays **−9.81**. Extra heft on jumps is the multiplier on the motor, applied as additional downward acceleration.

| Field | Start | Acceptable | Feel note |
|---|---:|---|---|
| `maxGroundSpeed` | 7 | 5–9 | Full run is readable against a 2 m obstacle |
| `groundAcceleration` | 18 | 12–30 | About 0.4 s to full speed |
| `groundDeceleration` | 24 | 14–40 | Release-to-stop is quicker than the windup |
| `turnResponsiveness` | 8 | 4–16 | Higher catches the stick faster. 8 slides through a 90° turn |
| `airControlMultiplier` | 0.35 | 0.2–0.6 | Can correct, cannot hover-steer |
| `slopeLimit` | 50 | 40–55 | Degrees |
| `jumpSpeed` | 7.5 | 6–10 | Initial upward speed on press. Full-hold apex clears a ~1.2 m bar, not a storey |
| `riseGravityMultiplier` | 1.7 | 1.2–2.5 | Total gravity scale while rising and the button is held (full-height jump) |
| `lowJumpGravityMultiplier` | 4 | 3–6 | Total gravity scale while rising after an early release. Cuts the hop short for a tap |
| `fallGravityMultiplier` | 2.4 | 1.8–3.2 | Total gravity scale while falling, same pattern as the rise scales |
| `coyoteTime` | 0.08 | 0.05–0.12 | |
| `jumpBufferTime` | 0.10 | 0.06–0.15 | |
| `diveSpeed` | 12 | 9–16 | Clearly faster than the run |
| `diveUpImpulse` | 2.5 | 1–4 | Lip clearance, not a second jump |
| `diveDuration` | 0.55 | 0.35–0.8 | |
| `diveCooldown` | 0.40 | 0.2–0.7 | Measured from the end of the dive state, so it covers the get-up too |
| `diveRecoverSpeed` | 2 | 1–3.5 | Slide speed under which the runner gets up on their own. Must be nearly settled |
| `divePitch` | 75 | 55–90 | Degrees of forward pitch for the belly dive. Near 90 the body lies flat on the slide |
| `divePitchTime` | 0.12 | 0.06–0.25 | Seconds for the body to reach the full pitch. Snappy, or the dive reads as a stumble |
| `getUpDuration` | 0.30 | 0.2–0.5 | Slerp from flat to upright after the slide |
| `grabRange` | 1.1 | 0.8–1.5 | From chest origin |
| `grabBreakDistance` | 2.2 | 1.6–3 | |
| `cameraDistance` | 5.5 | 4–8 | |
| `cameraHeight` | 2.2 | 1.5–3.5 | |
| `cameraPositionDamp` | 0.12 | 0.05–0.25 | Seconds of smoothing |
| `cameraFov` | 60 | 55–70 | Degrees |
| `diveFovKick` | 8 | 0–12 | Added to FOV |
| `diveFovDuration` | 0.20 | 0.1–0.35 | |

## Animation

Fields on `RunnerAnimationProfile` (Presentation), behavior in [SPEC-012](specs/SPEC-012-runner-animation.md). The two cycle speeds are calibration, not feel: set each to the ground speed where that clip's feet stop sliding, and re-measure whenever a clip changes. Above `runCycleSpeed` the run plays faster instead of sliding.

| Field | Start | Acceptable | Feel note |
|---|---:|---|---|
| `walkCycleSpeed` | 0.85 | calibrate | Mixamo Walk root speed 1.68 × avatar human scale 0.51 |
| `runCycleSpeed` | 2.3 | calibrate | Mixamo Run root speed 4.47 × 0.51. At `maxGroundSpeed` the run plays ~3× for a scurry |
| `locomotionBlendDamp` | 0.08 | 0.03–0.15 | Seconds. Keeps the wind-up readable without lagging the stop |
| `airBlendSpeedBand` | 3 | 1.5–5 | m/s of vertical speed for full Rise or Fall pose; apex sits between |
| `airPoseDelay` | 0.1 | 0.05–0.15 | Seconds off the ground before a walk-off shows the fall pose. Jumps skip it |
| `stateCrossfade` | 0.08 | 0.04–0.15 | Seconds, every transition, no exit times |
| `maxTurnLean` | 12 | 0–20 | Degrees of roll into a turn |
| `leanFullLateralAccel` | 30 | 15–50 | m/s² of turn for full lean (a full-speed 90° turn is about 28) |
| `turnLeanResponsiveness` | 12 | 6–20 | Higher snaps the lean in and out faster |
| `landSquash` | 0.15 | 0–0.3 | Fraction of height lost on the hardest landing, volume preserved |
| `landSquashFullImpact` | 12 | 6–16 | m/s of fall speed for full squash. A full-hold jump lands near 7 |
| `landSquashDuration` | 0.18 | 0.1–0.3 | Seconds to recover |

## Body size

Gameplay capsule: **radius 0.45 m**, **height 1.8 m** (Unity height includes the caps), center at `(0, 0.9, 0)` on a root at the feet. Mass **1**. Self-motion uses velocity change, so this mass only affects collisions with other bodies. Cosmetic mesh may be wider than the capsule; it does not collide.

Rigidbody interpolation **Interpolate**. Collision detection **Continuous Dynamic**. Max depenetration velocity stays at the project default until a Gym test shows tunneling or explosive pops.

## Playtest questions

Ask these in the Gym before calling a milestone fun:

- Can you stop on a 1 m platform after a full run, on purpose, three times in a row?
- Does a tap-jump and a full hop feel like the same verb at two heights, with the full hop still short?
- Does the dive feel faster than running the same straight, and scary to use near an edge?
- After a dive, are you back on your feet and steering again without touching the stick?
- Can a new watcher point at the safe line without playing?

If an answer is no, change a Start value or a spec behavior. Do not add a new verb to paper over it.
