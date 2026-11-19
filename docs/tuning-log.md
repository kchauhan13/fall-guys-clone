# Tuning log

Append a row when a value in [game-feel.md](game-feel.md) changes. The Start column in that file is the current value. This log is why it changed.

| Date | Field | From | To | Scene | Reason |
|---|---|---:|---:|---|---|
| 2026-10-19 | `stumbleSpeedThreshold` | 8 | 10 | Gym | Runner knocked down by ordinary movement bumps; raised above run speed (7) so only fast hits (dive, sweeper) stumble. |
| 2026-10-19 | `lowJumpGravityMultiplier` | (new) | 4 | Gym | New field for the variable-height (Mario-style) jump: gravity while rising after an early release, cutting a tap into a short hop. See SPEC-002. |
| 2026-10-19 | sweeper `angularSpeed` (Gym `StumbleTestBar`, not a profile field) | 2.5 | 3.25 | Gym | Peak sweep speed raised from ~10 to ~13 m/s to stay clearly above the new `stumbleSpeedThreshold`. |
| 2026-10-26 | `RunnerAnimationProfile` (all fields) | (new) | see game-feel.md, Animation | Gym | New presentation profile for SPEC-012 runner animation. Cycle speeds are first guesses pending real clips; calibrate per clip. |
| 2026-11-04 | `walkCycleSpeed` | 1.5 | 0.85 | Gym | Calibrated to the Mixamo Walk clip: root speed 1.68 m/s × avatar human scale 0.51. |
| 2026-11-04 | `runCycleSpeed` | 3.5 | 2.3 | Gym | Calibrated to the Mixamo Run clip: root speed 4.47 m/s × 0.51. |
| 2026-11-04 | Physics `autoSimulation` (project setting, not a profile field) | off | on | Gym | Was off with no script calling `Physics.Simulate`, so the runner never moved. Fixed step 0.02 and gravity unchanged. |
