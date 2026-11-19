# ADR-0002 — Hybrid rigidbody motor

- Status: accepted
- Date: 2026-10-05

## Context

A kinematic character controller is easy to make precise and awkward to shove, flop, and pile. A fully loose ragdoll is funny and hard to steer. Tumble needs both, in different states, on one body.

## Decision

One dynamic rigidbody and one capsule.

- `Upright` and the upright part of a jump: rotation locked on X and Z, horizontal velocity driven toward a wish velocity.
- `Diving`, `Stumbled`, and `Grabbing`: rotation unlocked, physics contacts matter more than the stick.
- `GettingUp`: scripted return to upright, then the locks come back.

Self-motion uses velocity change so profile numbers stay in m/s. Mass is for collisions only. Starting mass is 1.

## Consequences

- Hits can come from any obstacle with a rigidbody. No separate "knockback script" per hazard.
- The motor must explicitly manage constraints when states change, or the runner will pop or stay frozen.
- Network prediction later applies to this same state machine. We do not maintain a kinematic controller beside it.
- `CharacterController` is not used for the runner.
