# ADR-0005 — Input System at the start of M1

- Status: accepted
- Date: 2026-10-05

## Context

The project still uses the legacy Input Manager. `LocalCommandSource` needs actions with clear edges (jump, dive) and levels (move, look, grab). Rebinding and a second local player later are easier if that mapping is an asset from the start.

## Decision

When M1 begins, install the Input System package on the **1.7.x** line compatible with Unity 2021.3. Set Active Input Handling to **Both** so the editor itself keeps working.

Gameplay reads an Input Actions asset in `Assets/_Project/Settings/`. Action map `Runner`:

| Action | Type | Binding (keyboard / mouse) |
|---|---|---|
| Move | Vector2 | WASD and arrows |
| Look | Vector2 | Mouse delta |
| Jump | Button | Space |
| Dive | Button | Left Shift |
| Grab | Button | E, hold |

Gamepad bindings can be added in the same asset when the Gym is playable on a controller. They are not a gate for M1.

`UnityEngine.Input` is not used in new gameplay scripts.

## Consequences

- The first M1 task is the package and the asset, before the motor.
- UI and editor tools may still see the old input module. That is fine.
- Input System 1.11 and newer are avoided if Package Manager marks them incompatible with this editor.
