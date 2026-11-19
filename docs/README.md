# Docs

Source of truth for Tumble. Code follows these files. If a playtest changes a decision, edit the doc in the same change as the asset or script.

## Read first

| Doc | Role |
|---|---|
| [STATUS.md](STATUS.md) | What agents are allowed to build right now |
| [vision.md](vision.md) | Pillars, fantasy, session size, non-goals |
| [game-feel.md](game-feel.md) | How control should feel, and the only copy of tuning numbers |
| [contracts.md](contracts.md) | Types and fields shared by every spec |
| [architecture.md](architecture.md) | Systems, folders, physics layers, dependency rules |
| [milestones.md](milestones.md) | Order of work and the gate between them |

## Use when the task needs them

| Doc | Role |
|---|---|
| [specs/README.md](specs/README.md) | Spec list and the required spec format |
| [ai-workflow.md](ai-workflow.md) | How an agent implements one spec in Unity and Blender |
| [content-pipeline.md](content-pipeline.md) | Blender export and collider rules |
| [glossary.md](glossary.md) | Shared names |
| [tuning-log.md](tuning-log.md) | History of feel changes |
| [decisions/](decisions/) | Why the big technical choices were made |

## Decisions

| ADR | Choice |
|---|---|
| [0001](decisions/ADR-0001-simulation-presentation-split.md) | Simulation and presentation are separate |
| [0002](decisions/ADR-0002-hybrid-rigidbody-motor.md) | Hybrid rigidbody motor |
| [0003](decisions/ADR-0003-defer-netcode.md) | No netcode until movement is fun |
| [0004](decisions/ADR-0004-render-pipeline.md) | Stay on the built-in pipeline through the prototype |
| [0005](decisions/ADR-0005-input-system.md) | New Input System, adopted at the start of M1 |
| [0006](decisions/ADR-0006-art-versus-collision.md) | Blender makes visuals; Unity primitives collide |
| [0007](decisions/ADR-0007-authority-model.md) | Future multiplayer is server-authoritative physics |

## Rule for numbers

Starting and current feel values live only in [game-feel.md](game-feel.md). Specs describe behavior. The tuning log records each change. Do not copy a speed or a timer into a second doc.
