# ADR-0004 — Built-in pipeline through the prototype

- Status: accepted
- Date: 2026-10-05

## Context

This project is on the built-in render pipeline in Unity 2021.3.25f1. There are no art assets yet. URP is the better long-term home for a stylized game, and it is cheaper to adopt before materials exist than after. It is also a distraction from the motor.

## Decision

Stay on the built-in pipeline for M0–M6. Blockout materials are simple albedo colors from [content-pipeline.md](../content-pipeline.md).

Reopen this ADR before a second course gets real art, or before anyone authors more than a handful of materials. The likely follow-up is URP 12.x, which supports 2021.3. That migration is its own task, not a side effect of a gameplay spec.

## Consequences

- Shaders and camera post effects stay compatible with the built-in pipeline.
- Agents do not install URP as part of M1.
- Visual targets in the feel doc are about silhouette and readability, not a specific toon shader.
