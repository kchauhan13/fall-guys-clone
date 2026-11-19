# ADR-0007 — Server-authoritative physics, chosen in M6

- Status: accepted
- Date: 2026-10-05

## Context

Eight dynamic runners plus kinematic obstacles will not stay in lockstep across machines. PhysX is not cross-platform deterministic here, even with enhanced determinism enabled. A party game can tolerate a little correction. It cannot tolerate divergent qualify results.

## Decision

The future model is:

- The server simulates every motor and obstacle from commands and the round clock.
- The local client predicts its own motor and reconciles to snapshots.
- Remote runners are interpolated. They are not locally simulated as the authority.
- Qualification is server state replicated on the snapshot.

Lockstep and "everyone simulates everything" are rejected.

The library (FishNet, Photon Fusion, or Netcode for GameObjects) is picked during the M6 spike against these requirements:

1. Runs on Unity 2021.3.25f1.
2. Can host 8 dynamic rigidbodies with client-side prediction for the owner.
3. Can drive kinematic obstacles from a shared clock.
4. Has a dedicated or listen server so qualify logic has one owner.

The spike writes ADR-0008 with the winner. Until that file exists, no library is the project standard.

## Consequences

- Commands and snapshots stay small and blittable-friendly: structs, explicit enum values.
- Presentation effects are not gameplay authority.
- We do not turn on `m_EnableEnhancedDeterminism` as a substitute for authority.
- Session size stays 8 until a later ADR raises it.
