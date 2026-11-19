# Vision

Working title: **Tumble**. Rename it in [STATUS.md](STATUS.md); the code name `Tumble` stays until a rename task says otherwise.

## Fantasy

You are a rounded runner on a toy obstacle course. Speed takes a moment to build and a moment to lose. The dive is a commitment. A swinging bar can send you tumbling, and getting up is fast enough that a good lead still matters. Crossing the line feels half skill and half delightful accident.

## Pillars

1. **Momentum you can feel.** Acceleration, sliding turns, and the dive all commit the body. Stopping is a choice with a cost.
2. **Bodies are the comedy.** Hits, piles, and recoveries read in silhouette. The joke is physical, short, and recoverable.
3. **The route is readable.** A new player can see the safe line in a second. Chaos comes from runners and moving props, not from hidden pits or maze geometry.

If a feature serves none of these, it waits.

## Player verbs

In order of teaching:

| Verb | Role |
|---|---|
| Move | Build and steer momentum |
| Jump | Short hop over low gaps and sweeps |
| Dive | Burst of speed and a slide; the main skill expression |
| Recover | Get up after a stumble without a long cutscene |
| Grab | Catch another runner or a ledge-like prop; introduced after the race verbs feel good |

The body is the whole moveset. There are no weapons, inventories, or score combos in the prototype.

## What a round is

A round lasts about 60–90 seconds. Up to **8** runners share a course that funnels, opens, and funnels again. The first half of the field to reach the finish qualifies. Everyone else is out for that round.

Eight is the session-size target so collisions stay personal and a later physics server can simulate every runner. Raising it is a vision change, recorded in an ADR, because interest management and remote bodies get harder past this.

## Reference, and what we leave behind

Fall Guys is the feel reference for weight, dive commitment, readable toy courses, and short elimination rounds. Human Fall Flat is a reference for clumsy grabbing. Neither game's characters, names, level layouts, audio, or UI are used.

We are not making a precision platformer. Jumps are readable hops. Mastery is choosing when to dive and how to recover, not frame-perfect landing.

## Prototype non-goals

Cosmetics, accounts, matchmaking, parties, chat, a level editor, more than one finished round, and any network code. Those start only when [STATUS.md](STATUS.md) names their milestone.

## Originality

Courses, the runner silhouette, obstacle shapes, and UI are original. Blockout color is for readability (floor, hazard, finish), not a copy of another game's palette.
