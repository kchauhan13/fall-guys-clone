"""Author the runner clips the Mixamo set does not cover.

The Mixamo download gives Idle, Walk and Run. The dive, the get-up and the three
air poses are key-framed here against the same rig, in the same skeleton, so Unity
maps them onto SK_Runner's humanoid avatar exactly like the Mixamo clips.

Run head-less:

    Blender -b Models/pim-smiling-friends/source/SK_Runner.blend \
            --python Models/pim-smiling-friends/source/author_clips.py -- \
            Assets/_Project/Art/Runner/Animation [preview]

`preview` also writes PNGs of every pose next to the exports so the shapes can be
checked without opening Unity.

Rig facts (armature space, metres):
    forward = +Y, up = +Z, the character's left = -X, feet at z = 0.
"""

import math
import os
import sys

import bpy
from mathutils import Matrix, Quaternion, Vector

# ---------------------------------------------------------------- arguments

argv = sys.argv[sys.argv.index("--") + 1:] if "--" in sys.argv else []
OUT_DIR = argv[0] if argv else "Assets/_Project/Art/Runner/Animation"
PREVIEW = "preview" in argv[1:]

FPS = 30
PREFIX = "A_Runner_"

arm = bpy.data.objects["SK_Runner"]
bpy.context.view_layer.objects.active = arm
arm.select_set(True)

# ---------------------------------------------------------------- pose helpers


def parent_delta(pb):
    """How far the bone's parent has turned away from its rest orientation."""
    if pb.parent is None:
        return Matrix.Identity(3)
    p = pb.parent
    return p.matrix.to_3x3() @ p.bone.matrix_local.to_3x3().inverted()


def aim(pb, target, twist=None):
    """Point the bone's head-to-tail axis along `target`, in armature space.

    `twist` optionally names the direction the bone's own X axis should take, so an
    arm can reach forward without spinning around its own length.
    """
    bpy.context.view_layer.update()

    y = Vector(target).normalized()
    if twist is None:
        x = pb.matrix.to_3x3() @ Vector((1.0, 0.0, 0.0))
    else:
        x = Vector(twist)
    x = (x - y * x.dot(y))
    if x.length < 1e-5:
        x = Vector((1.0, 0.0, 0.0)) - y * y.x
    x.normalize()
    z = x.cross(y)

    rotation = Matrix((x, y, z)).transposed()
    current = pb.matrix.copy()
    pb.rotation_mode = "QUATERNION"
    pb.matrix = Matrix.Translation(current.to_translation()) @ rotation.to_4x4()
    bpy.context.view_layer.update()

    check = (pb.matrix.to_3x3() @ Vector((0.0, 1.0, 0.0))).normalized()
    if check.dot(y) < 0.98:
        print("WARNING: %s aimed at %s but ended at %s" % (pb.name, tuple(y), tuple(check)))


def mirror(direction):
    """Mirror a direction across the character's centre line."""
    return (-direction[0], direction[1], direction[2])


# ---------------------------------------------------------------- the poses
#
# Directions are armature-space unit vectors for the bone's head-to-tail axis.
# The dive is authored in body space: the motor pitches the whole body, so the
# clip only opens the arms and lets the legs trail (SPEC-012 rule 7).

DIVING = {
    # Arms straight out in front, a little above the shoulder line.
    "upperarm.L": ((0.16, 0.88, 0.45), (0.0, 0.55, 0.83)),
    "lowerarm.L": ((0.10, 0.95, 0.29), (0.0, 0.55, 0.83)),
    "upperarm.R": (mirror((0.16, 0.88, 0.45)), mirror((0.0, 0.55, 0.83))),
    "lowerarm.R": (mirror((0.10, 0.95, 0.29)), mirror((0.0, 0.55, 0.83))),
    # A slight arch through the chest, so the dive reads as a shape and not a plank.
    "spine": ((0.0, 0.04, 1.0), None),
    "spine1": ((0.0, 0.20, 0.98), None),
    # The head lifts and looks where the dive is going.
    "neck": ((0.0, 0.22, 0.97), None),
    "head": ((0.0, 0.30, 0.95), None),
}

# The get-up runs while the motor slerps the body from flat to upright, so the
# clip only has to carry the limbs: reach, tuck, stand.
GET_UP = {
    0: dict(DIVING),
    5: {
        # Arms pull in under the chest, knees fold under the hips.
        "upperarm.L": ((0.30, 0.55, 0.78), (0.0, 0.55, 0.83)),
        "lowerarm.L": ((0.18, 0.80, 0.57), (0.0, 0.55, 0.83)),
        "upperarm.R": (mirror((0.30, 0.55, 0.78)), mirror((0.0, 0.55, 0.83))),
        "lowerarm.R": (mirror((0.18, 0.80, 0.57)), mirror((0.0, 0.55, 0.83))),
        "upperleg.L": ((0.22, 0.52, -0.83), None),
        "lowerleg.L": ((0.14, -0.20, -0.97), None),
        "upperleg.R": (mirror((0.22, 0.52, -0.83)), None),
        "lowerleg.R": (mirror((0.14, -0.20, -0.97)), None),
        "spine": ((0.0, 0.10, 0.99), None),
        "spine1": ((0.0, 0.26, 0.96), None),
        "neck": ((0.0, 0.20, 0.98), None),
        "head": ((0.0, 0.24, 0.97), None),
    },
    10: {
        # Half way up: a crouch, weight forward, hands near the knees.
        "upperarm.L": ((0.34, 0.30, 0.89), (0.0, 0.55, 0.83)),
        "lowerarm.L": ((0.26, 0.55, 0.79), (0.0, 0.55, 0.83)),
        "upperarm.R": (mirror((0.34, 0.30, 0.89)), mirror((0.0, 0.55, 0.83))),
        "lowerarm.R": (mirror((0.26, 0.55, 0.79)), mirror((0.0, 0.55, 0.83))),
        "upperleg.L": ((0.16, 0.20, -0.97), None),
        "lowerleg.L": ((0.10, -0.05, -0.99), None),
        "upperleg.R": (mirror((0.16, 0.20, -0.97)), None),
        "lowerleg.R": (mirror((0.10, -0.05, -0.99)), None),
        "spine": ((0.0, 0.05, 1.0), None),
        "spine1": ((0.0, 0.12, 0.99), None),
        "head": ((0.0, 0.10, 0.99), None),
    },
    13: {
        # Standing, knees soft, arms back down by the sides.
        "upperarm.L": ((0.99, 0.0, -0.12), (0.0, 1.0, 0.0)),
        "lowerarm.L": ((1.0, 0.0, -0.03), (0.0, 1.0, 0.0)),
        "upperarm.R": (mirror((0.99, 0.0, -0.12)), mirror((0.0, 1.0, 0.0))),
        "lowerarm.R": (mirror((1.0, 0.0, -0.03)), mirror((0.0, 1.0, 0.0))),
        "upperleg.L": ((0.0, 0.0, -1.0), None),
        "lowerleg.L": ((0.0, -0.11, -0.99), None),
        "upperleg.R": (mirror((0.0, 0.0, -1.0)), None),
        "lowerleg.R": (mirror((0.0, -0.11, -0.99)), None),
    },
}

# The air poses are single frames blended by vertical speed (SPEC-012 rule 5).
JUMP_RISE = {
    # Knees up in front, arms swung up and back.
    "upperleg.L": ((0.16, 0.56, -0.81), None),
    "lowerleg.L": ((0.10, -0.30, -0.95), None),
    "upperleg.R": (mirror((0.16, 0.56, -0.81)), None),
    "lowerleg.R": (mirror((0.10, -0.30, -0.95)), None),
    "upperarm.L": ((0.22, -0.20, 0.95), (0.0, -0.4, 0.9)),
    "lowerarm.L": ((0.14, -0.22, 0.96), (0.0, -0.4, 0.9)),
    "upperarm.R": (mirror((0.22, -0.20, 0.95)), mirror((0.0, -0.4, 0.9))),
    "lowerarm.R": (mirror((0.14, -0.22, 0.96)), mirror((0.0, -0.4, 0.9))),
    "spine": ((0.0, -0.05, 1.0), None),
    "spine1": ((0.0, -0.10, 0.99), None),
}

JUMP_APEX = {
    # A tall, open shape at the top of the arc.
    "upperleg.L": ((0.13, 0.16, -0.98), None),
    "lowerleg.L": ((0.08, -0.05, -0.99), None),
    "upperleg.R": (mirror((0.13, 0.16, -0.98)), None),
    "lowerleg.R": (mirror((0.08, -0.05, -0.99)), None),
    "upperarm.L": ((0.30, -0.05, 0.95), (0.0, 0.9, 0.4)),
    "lowerarm.L": ((0.22, -0.02, 0.97), (0.0, 0.9, 0.4)),
    "upperarm.R": (mirror((0.30, -0.05, 0.95)), mirror((0.0, 0.9, 0.4))),
    "lowerarm.R": (mirror((0.22, -0.02, 0.97)), mirror((0.0, 0.9, 0.4))),
}

JUMP_FALL = {
    # Reaching for the ground, legs forward and apart.
    "upperleg.L": ((0.16, 0.44, -0.88), None),
    "lowerleg.L": ((0.10, 0.14, -0.98), None),
    "upperleg.R": (mirror((0.16, 0.44, -0.88)), None),
    "lowerleg.R": (mirror((0.10, 0.14, -0.98)), None),
    "upperarm.L": ((0.26, 0.42, 0.87), (0.0, 0.9, 0.4)),
    "lowerarm.L": ((0.18, 0.55, 0.81), (0.0, 0.9, 0.4)),
    "upperarm.R": (mirror((0.26, 0.42, 0.87)), mirror((0.0, 0.9, 0.4))),
    "lowerarm.R": (mirror((0.18, 0.55, 0.81)), mirror((0.0, 0.9, 0.4))),
    "spine": ((0.0, 0.02, 1.0), None),
    "spine1": ((0.0, 0.06, 1.0), None),
}

CLIPS = {
    "Dive": {0: DIVING},
    "GetUp": GET_UP,
    "JumpRise": {0: JUMP_RISE},
    "JumpApex": {0: JUMP_APEX},
    "JumpFall": {0: JUMP_FALL},
}


# ---------------------------------------------------------------- building


def clear_pose():
    for pb in arm.pose.bones:
        # Everything is keyed as a quaternion, so a bone never has to be re-interpreted
        # when the rotation mode changes.
        pb.rotation_mode = "QUATERNION"
        pb.rotation_quaternion = (1.0, 0.0, 0.0, 0.0)
        pb.location = (0.0, 0.0, 0.0)
        pb.scale = (1.0, 1.0, 1.0)
    bpy.context.view_layer.update()


def apply_pose(pose):
    clear_pose()
    for name, spec in pose.items():
        pb = arm.pose.bones.get(name)
        if pb is None:
            raise KeyError("no bone named %s" % name)
        direction, twist = spec
        aim(pb, direction, twist)


def all_fcurves(action):
    """Blender 4.4 moved the curves into action layers and slots."""
    if hasattr(action, "fcurves"):
        return list(action.fcurves)
    curves = []
    for layer in action.layers:
        for strip in layer.strips:
            for bag in getattr(strip, "channelbags", []):
                curves.extend(bag.fcurves)
    return curves


def build_action(clip, frames):
    action = bpy.data.actions.new(clip)
    arm.animation_data_create()
    arm.animation_data.action = action
    scene = bpy.context.scene
    scene.render.fps = FPS
    scene.frame_start = frames[0]
    scene.frame_end = frames[-1]

    for frame in frames:
        apply_pose(CLIPS[clip][frame])
        for pb in arm.pose.bones:
            pb.keyframe_insert(data_path="rotation_quaternion", frame=frame)
        for pb in arm.pose.bones:
            pb.keyframe_insert(data_path="location", frame=frame)
            pb.keyframe_insert(data_path="scale", frame=frame)

    # Flatten the curves so the motion reads as a pose, not a wobble.
    for fc in all_fcurves(action):
        for kp in fc.keyframe_points:
            kp.interpolation = "BEZIER"
    return action


def export(clip, frames):
    arm.animation_data.action = bpy.data.actions[clip]
    path = os.path.join(OUT_DIR, PREFIX + clip + ".fbx")
    bpy.ops.export_scene.fbx(
        filepath=path,
        use_selection=True,
        object_types={"ARMATURE"},
        use_armature_deform_only=False,
        add_leaf_bones=False,
        bake_anim=True,
        bake_anim_use_all_bones=True,
        bake_anim_use_nla_strips=False,
        bake_anim_use_all_actions=False,
        bake_anim_force_startend_keying=True,
        bake_anim_step=1.0,
        axis_forward="-Z",
        axis_up="Y",
        apply_scale_options="FBX_SCALE_NONE",
        global_scale=1.0,
        use_space_transform=False,
        path_mode="AUTO",
        mesh_smooth_type="FACE",
        use_custom_props=False,
        use_metadata=False,
    )
    print("EXPORTED", path)


def preview(clip, frame):
    scene = bpy.context.scene
    scene.frame_set(frame)
    apply_pose(CLIPS[clip][frame])
    target = Vector((0.0, 0.0, 0.95))
    for tag, loc in (
        ("side", (4.8, 0.0, 1.4)),
        ("front", (0.0, 4.8, 1.4)),
        ("three", (3.4, 3.4, 1.9)),
    ):
        cam = scene.camera
        cam.location = loc
        cam.rotation_euler = (target - Vector(loc)).to_track_quat("-Z", "Y").to_euler()
        scene.render.filepath = "/tmp/tumble_clip_%s_%03d_%s.png" % (clip, frame, tag)
        bpy.ops.render.render(write_still=True)
        print("PREVIEW", scene.render.filepath)


def setup_preview():
    scene = bpy.context.scene
    scene.render.engine = "BLENDER_WORKBENCH"
    scene.display.shading.light = "STUDIO"
    scene.display.shading.studio_light = "Default"
    scene.display.shading.color_type = "MATERIAL"
    scene.display.shading.show_cavity = True
    scene.render.resolution_x = 420
    scene.render.resolution_y = 560
    scene.render.film_transparent = False
    scene.world = bpy.data.worlds.new("Preview")
    scene.world.color = (0.75, 0.80, 0.88)
    cam_data = bpy.data.cameras.new("PreviewCam")
    cam = bpy.data.objects.new("PreviewCam", cam_data)
    scene.collection.objects.link(cam)
    cam.location = (3.4, -0.4, 1.1)
    cam.rotation_euler = (math.radians(84.0), 0.0, math.radians(90.0))
    scene.camera = cam
    return cam


os.makedirs(OUT_DIR, exist_ok=True)
if PREVIEW:
    setup_preview()

for clip, frames in CLIPS.items():
    build_action(clip, sorted(frames))
    export(clip, sorted(frames))
    if PREVIEW:
        for frame in sorted(frames):
            preview(clip, frame)

print("DONE")
