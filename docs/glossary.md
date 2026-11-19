# Glossary

Use these names in specs, scripts, and Blender files.

| Term | Meaning |
|---|---|
| Runner | The player body. A capsule in gameplay. The visible mesh is cosmetic. |
| Motor | `PlayerMotor`. Turns a `PlayerCommand` into rigidbody motion. |
| Command | One tick of intent: stick, look yaw, and action edges. |
| Profile | A `MovementProfile` ScriptableObject. Holds feel numbers. |
| Upright | Running and jumping with pitch and roll locked. |
| Dive | The committed burst. Fastest move, weakest control. |
| Stumble | Unlocked rotation after a hard hit or a bad landing. |
| Get-up | Short recovery from Stumble back to Upright. |
| Grab | A reach that joints the runner to another body. |
| Gym | The sandbox scene where feel is tested. No round rules. |
| Round | A timed course with a qualify line. |
| Qualify | Reaching the finish volume before the cutoff. |
| Round clock | Gameplay time and tick. Obstacle motion reads this, not `Time.time`. |
| PlayerId | Integer identity of a runner. The motor never assumes there is only one. |
| Blockout | Greybox mesh from Blender. Gameplay still uses Unity primitive colliders. |
| Presentation | Camera, animation, VFX, audio, HUD. Reads simulation. Never writes it. |
