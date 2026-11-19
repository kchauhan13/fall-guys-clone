# Content pipeline

Blender builds the visible shape. Unity builds the gameplay shape. A pretty mesh that is also the collider will fight the motor, because triangle meshes catch the capsule and make movement feel random.

## Scale and axes

- 1 Blender unit = 1 meter = 1 Unity unit.
- Apply scale and rotation in Blender before export (`Ctrl+A` → Rotation & Scale).
- FBX export: **Y up**, **−Z forward**, scale 1, selected objects only.
- The runner's game root sits at the feet. Blender blockout of the runner uses the same origin.

## What belongs in Blender

- Blockout and final meshes
- A face and body that read at camera distance (the capsule is only 1.8 m tall)
- Pivot at the base for props that sit on the floor
- Pivot at the spin center for bars and hammers

## What stays in Unity

- Capsule, box, and sphere colliders
- Rigidbodies, joints, and layers from [contracts.md](contracts.md)
- Materials that mark gameplay role
- The `RoundClock` driver on moving obstacles

Imported meshes on runners and obstacles have colliders removed on import. If a mesh collider is required for a non-gameplay background, it is on the `Cosmetic` layer and is static.

## Blockout colors

Use these on Unity materials so every course reads the same way in greybox. Hex values are the albedo.

| Role | Color | Material name |
|---|---|---|
| Ground you can stand on | `#C8C2B4` | `MAT_Blockout_Ground` |
| Hazard that moves or hurts | `#E07A3D` | `MAT_Blockout_Hazard` |
| Finish / qualify | `#3DDC97` | `MAT_Blockout_Finish` |
| Non-interactive dressing | `#8E8E93` | `MAT_Blockout_Dressing` |

## Naming

| Asset | Pattern | Example |
|---|---|---|
| Static mesh | `SM_` | `SM_Platform_4x4` |
| Skinned mesh | `SK_` | `SK_Runner` |
| Prefab | role name | `Runner`, `Spinner` |
| Collider child | `COL_` | `COL_Capsule` |

File names match object names. No `Cube.001`.

## Where files go

Export FBX into `Assets/_Project/Art/Blockout/` or `Assets/_Project/Art/Runner/`. Keep `.blend` sources in a folder outside `Assets/` if the file gets heavy, and copy the FBX in. A `.blend` inside `Assets/` is allowed for a small blockout because Unity 2021 imports it, but the gameplay prefab still uses Unity colliders.

## Blender MCP

Use Blender for mesh and layout. After export, check the Unity import: scale 1, no unexpected rotation, collider removed from the gameplay object. Do not author the motor, the joint, or the qualify volume in Blender.

## Runner model and clips

`SK_Runner.fbx` and `T_Runner_Pim.png` live in `Assets/_Project/Art/Runner/`. The source is `Models/pim-smiling-friends/source/SK_Runner.blend`, outside `Assets/`: scaled to 1.8 m, feet at the origin, facing −Y in Blender (+Z in Unity), T-pose rest, at most 4 bone weights per vertex. Export with armature and mesh selected, no leaf bones, deform bones only, no baked animation.

Clips go in `Assets/_Project/Art/Runner/Animation/` as `A_Runner_<Clip>.fbx`, with `<Clip>` one of `Idle`, `Walk`, `Run`, `JumpRise`, `JumpApex`, `JumpFall`, `Dive`, `GetUp`. Mixamo downloads: FBX, "Without Skin", **In Place** checked for walk and run, then rename the file. The `.fbx` extension is required: a file without it imports as an unknown asset and is ignored. `RunnerAssetPostprocessor` sets Humanoid and looping on first import. It bakes root rotation (Body Orientation) and height into the pose and leaves horizontal travel on the root, where root-motion-off throws it away. A clip that isn't In Place therefore still stays over the capsule. Then run **Tumble → Build Runner Animator** to rebuild `AC_Runner` and re-link the prefab. If a clip was imported before these rules existed, run **Tumble → Reapply Runner Clip Import Rules** first. A clip that is exported from `SK_Runner.blend` needs no In Place step: key only the bones, leave the object transform alone, export the armature only with baked animation, no leaf bones, Y up, −Z forward. A file with no animation curves imports without error but plays as a broken pose, so check that the clip in Unity has a nonzero length and is humanoid.

## Runner visual

The first Gym can use a Unity capsule mesh as the visual. Replace it with `SK_Runner` when a Blender pass happens. The capsule collider dimensions in [game-feel.md](game-feel.md) do not change to match the mesh. If the mesh is wider than the collider, that is intentional: the body reads big, the collision stays fair.
