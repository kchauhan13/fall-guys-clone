# SPEC-005 — Grab

- Status: deferred
- Milestone: M4
- Depends on: [SPEC-004](SPEC-004-stumble.md)

Tighten this spec at the start of M4, before writing the joint.

## Fantasy

You reach, catch another body, and both of you swing until someone lets go or the link snaps. It is a way to spoil a dive or save a fall, not a climb of the whole course.

## In scope

A hold to grab a runner or a `Grabbable` collider in front of `GrabOrigin`. A configurable joint. Break on release, distance, or a non-grab stumble.

## Out of scope

Climbing arbitrary level geometry, grabbing the finish line, throwing with a charge meter.

## Rules

1. `GrabHeld` while upright or diving casts from `GrabOrigin` forward by `grabRange`. The first `Runner` or `Grabbable` hit becomes the target.
2. On hit, state is `Grabbing`, a joint connects the bodies, and `Grabbed` fires.
3. Releasing the button, exceeding `grabBreakDistance`, or entering `Stumbled` from a threshold impact destroys the joint and raises `GrabBroke`.
4. The grabbed runner is not controlled by the grabber's stick. Both motors keep simulating. The joint is the only coupling.
5. A miss does nothing: no cooldown beyond the next tick's cast.

## Acceptance

- [ ] A dummy runner in the Gym can be grabbed and dragged for a short distance.
- [ ] Releasing the button separates you within a moment.
- [ ] A fast obstacle hit breaks the grab and stumbles the struck runner.
- [ ] You cannot grab the floor or the finish volume.

## Open questions

- Whether a dive grab should yank harder than an upright grab. Decide with a Gym test at M4, then write the number into `game-feel.md` if the answer is yes.
