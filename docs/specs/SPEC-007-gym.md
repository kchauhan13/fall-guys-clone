# SPEC-007 — Gym

- Status: tuning
- Note: M1 stations done. M2 adds a dive-timing straight and a clock-driven test bar (see SPEC-004 rule 6).
- Milestone: M1
- Depends on: [architecture](../architecture.md), [content pipeline](../content-pipeline.md)

## Fantasy

A private course where one verb can be repeated until it feels right. No timer, no elimination, no menu.

## In scope

`Assets/_Project/Scenes/Gym.unity`. Flat ground, a 1 m stop platform, a 45° slope, a 70° wall, a 0.5 m bar, a 2.5 m wall, a lip for coyote tests, a kill volume that respawns at a checkpoint, and a camera orbit good enough for M1. Lighting and a ground plane so the scene is readable in play mode.

M2 adds the variable-speed bar beside this layout. M3 adds the clock-driven obstacle. Those land in their own specs; leave space in the scene rather than building them in M1.

## Out of scope

Round timer, qualify UI, a second human, art beyond blockout colors.

## Rules

1. Pressing Play with the Gym open spawns one runner at `PlayerId` 0 with the default `MovementProfile`.
2. The profile asset lives at `Assets/_Project/ScriptableObjects/Movement/DefaultMovementProfile.asset` and is the asset designers edit.
3. Blockout materials follow [content-pipeline.md](../content-pipeline.md).
4. A `CourseTrigger` volume below the floor respawns the runner at the Gym checkpoint with upright state and zero velocity. Full fall rules are [SPEC-008](SPEC-008-fall-respawn.md). M1's version can be the same component with checkpoints only.
5. The scene has a camera and a directional light.
6. SampleScene is not modified.

## Acceptance

- [ ] Play from the Gym with no other scenes loaded. The runner spawns and can attempt the M1 checks in SPEC-001 and SPEC-002.
- [ ] Falling off returns you to the checkpoint.
- [ ] Quitting play mode leaves the editor in edit mode with a clean console.

## Open questions

None.
