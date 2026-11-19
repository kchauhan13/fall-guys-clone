---
working_title: Tumble
unity_version: 2021.3.25f1
render_pipeline: built-in
active_milestone: M2
next_milestone: M3
implementation_allowed: true
netcode: deferred
session_size_target: 8
---

# Status

This file is the on/off switch for implementation. If it disagrees with another doc, this file wins until a human updates it.

## Now

**M2 — Dive and stumble — is implemented and awaiting the human feel playtest.** Specs: [SPEC-003](specs/SPEC-003-dive.md), [SPEC-004](specs/SPEC-004-stumble.md).

M1 passed its feel playtest and is done.

Done so far in M2:

- `PlayerMotor` now runs the full state machine: `Diving` (burst speed, weak steer, cooldown, upward kick), `Stumbled` (rotation unlocked, physics owns the body), and `GettingUp` (slerp upright, then locks return), all timed off `RoundClock`.
- Dive input path from `Upright`, and a collision-impact path (`OnCollisionEnter` + `stumbleMask`, Ground excluded) that stumbles from any state above the threshold.
- `StumbleTestBar` — a kinematic sweeper driven by the clock (not `Time.time`) — plus a dive-timing straight, added to the Gym and wired into `GymBootstrap`.
- Runner prefab `stumbleMask` set to Runner/Obstacle/Grabbable.
- 13 Edit Mode tests still pass. Deterministic play-mode probe confirmed: over one dive window a dive covers 6.72 m vs 2.63 m running; re-diving during cooldown is blocked and allowed after; a dive into the wall stumbles then recovers upright in 0.98 s (get-up begins ~0.65 s after the hit); the clock-driven sweeper stumbles a runner in its path through the real collision path.

Tuning pass (2026-10-19, from playtest feedback), verified by probe:

- **Variable-height jump (Mario-style).** `PlayerCommand.JumpHeld` added; the motor now holds the lighter `riseGravityMultiplier` while the button is held and switches to the new `lowJumpGravityMultiplier` (4) on early release, latched for the rest of the rise. Tap 0.73 m, medium 1.26 m, full hold 1.61 m (capped). SPEC-002 updated and back to `tuning`.
- **Fewer accidental knockdowns.** `stumbleSpeedThreshold` 8 -> 10 (above run speed 7); the Gym sweeper raised to ~13 m/s peak so it still stumbles. Running into a wall at run speed no longer knocks the runner down; the sweeper still does. Logged in [tuning-log.md](tuning-log.md).

Remaining before advancing to M3:

1. Human re-tests `Assets/_Project/Scenes/Gym.unity`: the variable jump feels right (tap vs hold, sensible max), ordinary movement no longer knocks you down, dive still commits, and the sweeper knock-down + ~1 s recovery feels right.
2. Log any further feel changes in [game-feel.md](game-feel.md) + [tuning-log.md](tuning-log.md).
3. Then set `active_milestone` to M3 and move the M2 specs (and SPEC-002) to `done`.

## Next

**M3 — Camera, fall, one real obstacle.** Specs: [SPEC-006](specs/SPEC-006-camera.md), [SPEC-008](specs/SPEC-008-fall-respawn.md), [SPEC-009](specs/SPEC-009-obstacle-clock.md). Do not start until the M2 gate passes and this file names M3.

## Off limits until their milestone is named here

- Dive, stumble, grab, camera polish, obstacles, round flow, UI, netcode, matchmaking, cosmetics.
- Changing the render pipeline.
- Replacing the fixed timestep (`0.02`) or gravity (`-9.81`).

## How to advance

1. Finish the active milestone's acceptance checks.
2. Set `active_milestone` to the next id from [milestones.md](milestones.md).
3. Point `next_milestone` one step further.
4. Leave `implementation_allowed` as the human set it.
