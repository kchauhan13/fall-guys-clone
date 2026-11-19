# AI workflow

How to implement a spec with Unity and Blender open. The goal is one vertical change that the next session can trust.

## Before editing

1. Read [STATUS.md](STATUS.md). If `implementation_allowed` is false, stop after explaining what is blocked.
2. If the requested work is past `active_milestone`, stop and name the milestone that owns it.
3. Read that milestone in [milestones.md](milestones.md), then only its specs.
4. Read [contracts.md](contracts.md) and the feel rows the spec uses. Do not invent a parallel set of speeds.

## While editing

- One spec at a time. A shared type change belongs in `contracts.md` first.
- New scripts go in the folder layout in [architecture.md](architecture.md), inside the assembly that matches their job.
- Feel numbers are serialized on `MovementProfile` or `CameraProfile`, set to the Start column.
- Gameplay time comes from `RoundClock`.
- Player input is read only in `LocalCommandSource`.
- After every script compile, read the Unity console and fix errors before play mode.
- Use Unity 2021.3 APIs. If a member might be from a newer editor, check it before using it.
- Blender output follows [content-pipeline.md](content-pipeline.md).

## Proving it

Run the spec's checklist in the Gym, in play mode, with the local runner. Then leave the editor in edit mode.

For a logic change with no feel impact, an Edit Mode test is enough. For a feel change, the checklist matters more than the test.

## After it works

- Set the spec's status to `done` or leave it `tuning` if the gate is still open.
- If a Start number changed, update [game-feel.md](game-feel.md) and append [tuning-log.md](tuning-log.md).
- If a rule changed, edit the spec so it describes the behavior that shipped.
- Advance [STATUS.md](STATUS.md) only when the milestone gate passes.

## What agents tend to break

- Reading input inside `PlayerMotor`, which makes a server tick impossible later.
- Driving a spinner with `Time.time`, which desyncs the moment a clock exists.
- Putting the qualify UI in charge of who qualified, instead of a flag on the snapshot.
- Tuning by editing constants in a script after a profile already exists.
- Expanding into grab, rounds, or netcode because the motor file is already open.

## Spec format

New features copy the format in [specs/README.md](specs/README.md) and get the next id. Code for a feature without a spec is incomplete even if it runs.
