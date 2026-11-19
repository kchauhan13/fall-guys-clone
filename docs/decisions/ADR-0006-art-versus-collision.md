# ADR-0006 — Visual mesh and gameplay collider are different objects

- Status: accepted
- Date: 2026-10-05

## Context

Blender is available and good at the runner and the course dressing. PhysX gameplay for this motor wants a capsule and a few boxes. Mesh colliders on movers make grounding and dives depend on triangle luck.

## Decision

Blender exports visuals. Unity prefabs add primitive colliders on the layers in [contracts.md](../contracts.md). Import settings strip colliders from those meshes.

The runner collider stays the capsule in [game-feel.md](../game-feel.md) even after the skinned mesh exists.

## Consequences

- A mesh can overlap its collider. Gameplay fairness follows the primitive.
- Obstacle prefabs have a `Visual` child and a `COL_` child.
- Agents using Blender MCP do not "finish" an obstacle by relying on the imported mesh collider.
