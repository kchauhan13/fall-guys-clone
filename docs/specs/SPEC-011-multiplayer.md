# SPEC-011 — Multiplayer spike

- Status: deferred
- Milestone: M6
- Depends on: M1–M5 gates, [ADR-0007](../decisions/ADR-0007-authority-model.md)

This spec is a spike plan, not permission to add a package earlier. Rewrite the acceptance steps into concrete library calls only after [STATUS.md](../STATUS.md) names M6.

## Fantasy

Two people in the Gym can bump into each other. Each one steers only their own runner. The dive you practiced alone still works when someone else is in the way.

## In scope

Two instances. Host simulates both motors. Each client sends `PlayerCommand` for its `PlayerId`. Remote body is visible and collides. One obstacle driven by the host clock. A short note that becomes ADR-0008 naming the library.

## Out of scope

Matchmaking, relays beyond what the spike needs to connect two machines on a LAN, cosmetics, anti-cheat, reconnection, 8-player load tests, dedicated server deployment.

## Rules

1. `PlayerMotor` stays free of network types. An adapter implements `ICommandSource` for remote commands and sends the local command out.
2. The host owns `RoundClock` and the qualify flags.
3. The client predicts the local runner only.
4. Remote runners render from snapshots. They are not a second local motor treated as authority.
5. Input, camera, and HUD stay on the owning client.
6. The spike runs in the Gym so a failure is about networking, not about an unfinished round.

## Acceptance

- [ ] Two instances on one LAN: each keyboard or gamepad moves only its runner.
- [ ] A dive into the other runner moves both bodies.
- [ ] The obstacle phase matches on both screens within a small, noted error.
- [ ] Disconnecting the client does not throw inside `PlayerMotor`.
- [ ] ADR-0008 exists and names the library, the version, and what failed in the alternatives.

## Open questions

Library choice is the point of the spike. Do not close it in advance.
