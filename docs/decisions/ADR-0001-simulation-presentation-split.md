# ADR-0001 — Split simulation and presentation

- Status: accepted
- Date: 2026-10-05

## Context

The prototype is single-player. The game we want later has several physics bodies whose positions must be decided by one authority. Camera, VFX, and UI cannot be inside that authority or every effect becomes a network problem.

## Decision

`PlayerMotor` consumes `PlayerCommand` and writes the rigidbody. It has no reference to input devices, cameras, or UI.

`LocalCommandSource` and the camera live in presentation and app code. They observe `PlayerSnapshot` and motor moments.

## Consequences

- M1 has more types than a single `PlayerController` script.
- Juice is a subscriber, so it can be added without retuning physics.
- A second runner in the Gym does not need a second input implementation inside the motor.
- Agents must not "simplify" this by sampling input in the motor during the prototype.
