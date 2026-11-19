# SPEC-002 — Jump

- Status: tuning
- Milestone: M1
- Depends on: [SPEC-001](SPEC-001-locomotion.md)

## Fantasy

A variable-height hop like a classic platformer: a tap gives a low hop, holding floats you up to a capped maximum. The fall is faster than the rise.

## In scope

Single jump, coyote time, jump buffer, and variable height via hold (press sets speed, early release cuts the rise). Three gravity multipliers: rise (held), low-jump (released), fall.

## Out of scope

Double jump, wall jump, dive-cancel, a thrust that lets you keep rising past the capped apex.

## Rules

1. `JumpPressed` on a tick while grounded, or while coyote time remains, sets vertical velocity to `jumpSpeed` and raises `Jumped`.
2. Coyote time starts when the runner leaves ground without jumping, and lasts `coyoteTime`.
3. If `JumpPressed` arrives airborne with no coyote time left, it is remembered for `jumpBufferTime` and fires on the first grounded tick inside that window.
4. A jump spends the buffer and the coyote window. Another jump requires a new ground contact.
5. While upright, rising, and the jump came from a jump: if `JumpHeld` the extra downward acceleration is `(riseGravityMultiplier - 1)` times gravity; if the button has been released it is `(lowJumpGravityMultiplier - 1)` times gravity. The maximum height is the full-hold apex under the rise multiplier; the player cannot exceed it.
6. The release latches for the rest of that rise: once released while rising, a mid-air re-press does not restore the rise multiplier.
7. A rise not caused by a jump (for example a bounce) uses ordinary gravity. While upright and falling, the extra acceleration uses `fallGravityMultiplier`.
8. Dive and stumble states do not use these multipliers; they use ordinary gravity until those specs say otherwise.
9. Jump does not change horizontal velocity.

## Acceptance

- [ ] A quick tap makes a low hop; holding makes a clearly higher hop that clears the Gym's 0.5 m bar; neither clears the 2.5 m wall.
- [ ] Holding longer than the full ascent does not go higher than the capped apex.
- [ ] Walking off a platform and pressing jump a moment later still jumps (coyote).
- [ ] Pressing jump a moment before landing jumps on contact (buffer).
- [ ] Two jumps without a landing do not happen.

## Open questions

None.
