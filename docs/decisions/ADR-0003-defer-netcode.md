# ADR-0003 — Defer netcode until the motor is fun

- Status: accepted
- Date: 2026-10-05

## Context

Physics party games throw away their first movement code once networking arrives, unless the input and the clock were already isolated. They also stall if multiplayer is built before anyone likes the jump.

## Decision

No transport, lobby, or network behaviour through M5.

From the first script, the seams in [architecture.md](../architecture.md) are real: commands, snapshots, `PlayerId`, and `RoundClock`.

M6 is a spike in the Gym with two instances. It picks a library. It does not ship a matchmade game.

## Consequences

- The prototype cannot be played by two people online. That is accepted.
- M6 is allowed to reject the first library. The motor API should not change to fit a library; an adapter sits beside `ICommandSource`.
- Agents do not add Netcode for GameObjects, FishNet, or Photon "while we are here."
