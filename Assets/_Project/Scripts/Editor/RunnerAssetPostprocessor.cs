using System.Linq;
using UnityEditor;

namespace Tumble.Editor
{
    // First-import rules for the runner model and its clips. See docs/specs/SPEC-012-runner-animation.md
    // and docs/content-pipeline.md. Only touches assets whose settings are new, so manual tweaks survive.
    public sealed class RunnerAssetPostprocessor : AssetPostprocessor
    {
        public const string RunnerModelPath = "Assets/_Project/Art/Runner/SK_Runner.fbx";
        public const string ClipFolder = "Assets/_Project/Art/Runner/Animation/";
        public const string ClipPrefix = "A_Runner_";

        public static readonly string[] ClipNames =
            { "Idle", "Walk", "Run", "JumpRise", "JumpApex", "JumpFall", "Dive", "GetUp" };

        static readonly string[] LoopingClips =
            { "Idle", "Walk", "Run", "JumpRise", "JumpApex", "JumpFall", "Dive" };

        public static string ClipPath(string clip) => ClipFolder + ClipPrefix + clip + ".fbx";

        static bool TryGetClipName(string path, out string clip)
        {
            clip = null;
            if (!path.StartsWith(ClipFolder) || !path.EndsWith(".fbx"))
                return false;
            string file = System.IO.Path.GetFileNameWithoutExtension(path);
            if (!file.StartsWith(ClipPrefix))
                return false;
            clip = file.Substring(ClipPrefix.Length);
            return ClipNames.Contains(clip);
        }

        void OnPreprocessModel()
        {
            var importer = (ModelImporter)assetImporter;
            if (!importer.importSettingsMissing)
                return;

            bool isRunner = assetPath == RunnerModelPath;
            if (!isRunner && !TryGetClipName(assetPath, out _))
                return;

            importer.globalScale = 1f;
            importer.useFileScale = true;
            importer.bakeAxisConversion = true;
            importer.addCollider = false;
            importer.importCameras = false;
            importer.importLights = false;
            importer.animationType = ModelImporterAnimationType.Human;
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;

            if (isRunner)
            {
                importer.importAnimation = false;
                importer.importBlendShapes = true;
                importer.materialImportMode = ModelImporterMaterialImportMode.None;
            }
            else
            {
                importer.importAnimation = true;
                importer.importBlendShapes = false;
                importer.materialImportMode = ModelImporterMaterialImportMode.None;
            }
        }

        void OnPreprocessAnimation()
        {
            var importer = (ModelImporter)assetImporter;
            if (!importer.importSettingsMissing || !TryGetClipName(assetPath, out string clip))
                return;

            ModelImporterClipAnimation[] takes = importer.defaultClipAnimations;
            if (takes.Length == 0)
                return;

            // One clip per file, named after the file.
            ModelImporterClipAnimation take = takes[0];
            take.name = clip;
            take.loopTime = LoopingClips.Contains(clip);
            ApplyRootRules(take);
            importer.clipAnimations = new[] { take };
        }

        // The motor moves the body, so the Animator runs with root motion off.
        // Rotation and height stay baked into the pose (bob, crouch, dive arch).
        // Horizontal travel is NOT baked: it goes to the root, which root-motion-off discards.
        // Baking it would slide the mesh away from the capsule for clips that aren't "In Place".
        public static void ApplyRootRules(ModelImporterClipAnimation take)
        {
            // Body Orientation, not Original: some downloads store the root turned 180°, which
            // made the runner face backward. Body orientation always faces the way the body does.
            take.lockRootRotation = true;
            take.keepOriginalOrientation = false;
            take.lockRootHeightY = true;
            take.keepOriginalPositionY = true;
            take.lockRootPositionXZ = false;
            take.keepOriginalPositionXZ = false;
        }

        [MenuItem("Tumble/Reapply Runner Clip Import Rules")]
        public static void ReapplyClipRules()
        {
            foreach (string clip in ClipNames)
            {
                if (!(AssetImporter.GetAtPath(ClipPath(clip)) is ModelImporter importer))
                    continue;
                ModelImporterClipAnimation[] defaults = importer.defaultClipAnimations;
                ModelImporterClipAnimation[] takes = importer.clipAnimations;
                if (takes.Length == 0)
                    takes = defaults;
                if (takes.Length == 0)
                    continue;
                // A first import can leave a stale empty range behind. Take the file's own range then.
                if (takes[0].lastFrame <= takes[0].firstFrame && defaults.Length > 0)
                {
                    takes[0].firstFrame = defaults[0].firstFrame;
                    takes[0].lastFrame = defaults[0].lastFrame;
                }
                takes[0].name = clip;
                takes[0].loopTime = LoopingClips.Contains(clip);
                ApplyRootRules(takes[0]);
                importer.clipAnimations = new[] { takes[0] };
                importer.SaveAndReimport();
            }
        }
    }
}
