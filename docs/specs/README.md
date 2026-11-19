# Specs

One file per feature. Status is `draft`, `ready`, `in-progress`, `tuning`, `done`, or `deferred`.

| ID | Spec | Milestone | Status |
|---|---|---|---|
| 001 | [Locomotion](SPEC-001-locomotion.md) | M1 | done |
| 002 | [Jump](SPEC-002-jump.md) | M1 | tuning |
| 003 | [Dive](SPEC-003-dive.md) | M2 | tuning |
| 004 | [No knock-down](SPEC-004-stumble.md) | M2 | done |
| 005 | [Grab](SPEC-005-grab.md) | M4 | deferred |
| 006 | [Camera](SPEC-006-camera.md) | M3 | ready |
| 007 | [Gym](SPEC-007-gym.md) | M1 | done |
| 008 | [Fall and respawn](SPEC-008-fall-respawn.md) | M3 | ready |
| 009 | [Obstacle clock](SPEC-009-obstacle-clock.md) | M3 | ready |
| 010 | [Round session](SPEC-010-round-session.md) | M5 | deferred |
| 011 | [Multiplayer spike](SPEC-011-multiplayer.md) | M6 | deferred |
| 012 | [Runner animation](SPEC-012-runner-animation.md) | M2 (presentation side-track) | in-progress |

`ready` means the spec is written and waiting for its milestone. `deferred` means the design is directional and should be tightened when that milestone becomes active, before coding.

Build only rows whose milestone is `active_milestone` in [STATUS.md](../STATUS.md).

## Format for a new spec

```markdown
# SPEC-0XX — Name

- Status: draft
- Milestone: M?
- Depends on: SPEC-...

## Fantasy
One or two sentences a player would recognize.

## In scope
## Out of scope

## Rules
Numbered, testable behavior. Reference profile fields by name.
Do not paste numbers; they live in game-feel.md.

## Acceptance
- [ ] Checklist a person can run in a named scene.

## Open questions
Empty if the spec is ready.
```

Copy this into `docs/specs/SPEC-0XX-slug.md` and add a row to the table above.
