using System.IO;
using Tumble.Presentation;
using Tumble.Simulation;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Tumble.Editor
{
    // Builds AC_Runner from clips named by convention and wires SK_Runner into Runner.prefab.
    // Safe to run again: it regenerates the controller and re-links the prefab. See SPEC-012.
    public static class RunnerAnimatorBuilder
    {
        const string ArtFolder = "Assets/_Project/Art/Runner/";
        const string ControllerPath = ArtFolder + "AC_Runner.controller";
        const string MaterialPath = ArtFolder + "MAT_Runner_Pim.mat";
        const string TexturePath = ArtFolder + "T_Runner_Pim.png";
        const string ProfilePath = "Assets/_Project/ScriptableObjects/Animation/DefaultRunnerAnimationProfile.asset";
        const string MovementProfilePath = "Assets/_Project/ScriptableObjects/Movement/DefaultMovementProfile.asset";
        const string PrefabPath = "Assets/_Project/Prefabs/Runner/Runner.prefab";
        const string ModelChildName = "SK_Runner";
        const int CosmeticLayer = 13;

        [MenuItem("Tumble/Build Runner Animator")]
        public static void Build()
        {
            RunnerAnimationProfile profile = LoadOrCreateProfile();
            Material material = LoadOrCreateMaterial();
            AnimatorController controller = BuildController(profile, out float getUpLength);
            WirePrefab(controller, material, profile, getUpLength);
            AssetDatabase.SaveAssets();
            Debug.Log($"[Tumble] Runner animator built. Clips found: {FoundClipSummary()}");
        }

        static RunnerAnimationProfile LoadOrCreateProfile()
        {
            var profile = AssetDatabase.LoadAssetAtPath<RunnerAnimationProfile>(ProfilePath);
            if (profile != null)
                return profile;

            Directory.CreateDirectory(Path.GetDirectoryName(ProfilePath));
            profile = ScriptableObject.CreateInstance<RunnerAnimationProfile>();
            // Start column of docs/game-feel.md, "Animation". Edit the asset, not this code, when tuning.
            profile.walkCycleSpeed = 1.5f;
            profile.runCycleSpeed = 3.5f;
            profile.locomotionBlendDamp = 0.08f;
            profile.airBlendSpeedBand = 3f;
            profile.airPoseDelay = 0.1f;
            profile.stateCrossfade = 0.08f;
            profile.maxTurnLean = 12f;
            profile.leanFullLateralAccel = 30f;
            profile.turnLeanResponsiveness = 12f;
            profile.landSquash = 0.15f;
            profile.landSquashFullImpact = 12f;
            profile.landSquashDuration = 0.18f;
            AssetDatabase.CreateAsset(profile, ProfilePath);
            return profile;
        }

        static Material LoadOrCreateMaterial()
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (material != null)
                return material;

            material = new Material(Shader.Find("Standard")) { name = "MAT_Runner_Pim" };
            material.SetTexture("_MainTex", AssetDatabase.LoadAssetAtPath<Texture2D>(TexturePath));
            material.SetFloat("_Glossiness", 0.25f);
            AssetDatabase.CreateAsset(material, MaterialPath);
            return material;
        }

        static AnimationClip LoadClip(string name)
        {
            foreach (Object asset in AssetDatabase.LoadAllAssetsAtPath(RunnerAssetPostprocessor.ClipPath(name)))
            {
                if (asset is AnimationClip clip && !clip.name.StartsWith("__preview__"))
                    return clip;
            }
            return null;
        }

        static string FoundClipSummary()
        {
            var found = new System.Collections.Generic.List<string>();
            foreach (string name in RunnerAssetPostprocessor.ClipNames)
                found.Add(name + (LoadClip(name) != null ? " ok" : " missing"));
            return string.Join(", ", found);
        }

        static AnimatorController BuildController(RunnerAnimationProfile profile, out float getUpLength)
        {
            AssetDatabase.DeleteAsset(ControllerPath);
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);

            controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
            controller.AddParameter("LocomotionScale", AnimatorControllerParameterType.Float);
            controller.AddParameter("VerticalSpeed", AnimatorControllerParameterType.Float);
            controller.AddParameter("Airborne", AnimatorControllerParameterType.Bool);
            controller.AddParameter("Diving", AnimatorControllerParameterType.Bool);
            controller.AddParameter("GettingUp", AnimatorControllerParameterType.Bool);
            controller.AddParameter("GetUpScale", AnimatorControllerParameterType.Float);
            foreach (AnimatorControllerParameter p in controller.parameters)
            {
                if (p.name == "LocomotionScale" || p.name == "GetUpScale")
                    p.defaultFloat = 1f;
            }
            controller.parameters = controller.parameters;

            AnimatorStateMachine sm = controller.layers[0].stateMachine;

            AnimatorState locomotion = sm.AddState("Locomotion", new Vector3(300, 0));
            locomotion.motion = BlendTree(controller, "Locomotion", "Speed",
                ("Idle", RunnerAnimator.IdleCoord), ("Walk", RunnerAnimator.WalkCoord), ("Run", RunnerAnimator.RunCoord));
            locomotion.speedParameter = "LocomotionScale";
            locomotion.speedParameterActive = true;

            AnimatorState airborne = sm.AddState("Airborne", new Vector3(300, 120));
            airborne.motion = BlendTree(controller, "Airborne", "VerticalSpeed",
                ("JumpFall", -1f), ("JumpApex", 0f), ("JumpRise", 1f));

            AnimatorState dive = sm.AddState("Dive", new Vector3(600, 0));
            dive.motion = LoadClip("Dive");

            AnimationClip getUpClip = LoadClip("GetUp");
            AnimatorState getUp = sm.AddState("GetUp", new Vector3(600, 120));
            getUp.motion = getUpClip;
            getUp.speedParameter = "GetUpScale";
            getUp.speedParameterActive = true;
            getUpLength = getUpClip != null ? getUpClip.length : 1f;

            sm.defaultState = locomotion;
            float fade = profile.stateCrossfade;

            // Simulation state wins over ground/air: dive and get-up interrupt anything.
            AnyTo(sm, dive, fade, ("Diving", true));
            AnyTo(sm, getUp, fade, ("GettingUp", true));

            To(locomotion, airborne, fade, ("Airborne", true));
            To(airborne, locomotion, fade, ("Airborne", false));
            To(dive, locomotion, fade, ("Diving", false), ("GettingUp", false), ("Airborne", false));
            To(dive, airborne, fade, ("Diving", false), ("GettingUp", false), ("Airborne", true));
            To(getUp, locomotion, fade, ("GettingUp", false), ("Diving", false));

            EditorUtility.SetDirty(controller);
            return controller;
        }

        static Motion BlendTree(AnimatorController controller, string name, string parameter,
            params (string clip, float threshold)[] children)
        {
            var tree = new BlendTree
            {
                name = name,
                blendType = BlendTreeType.Simple1D,
                blendParameter = parameter,
                useAutomaticThresholds = false,
            };
            AssetDatabase.AddObjectToAsset(tree, controller);

            int added = 0;
            foreach (var (clipName, threshold) in children)
            {
                AnimationClip clip = LoadClip(clipName);
                if (clip == null)
                    continue;
                tree.AddChild(clip, threshold);
                added++;
            }
            return added > 0 ? tree : null;
        }

        static void AnyTo(AnimatorStateMachine sm, AnimatorState to, float fade, params (string p, bool v)[] conditions)
        {
            AnimatorStateTransition t = sm.AddAnyStateTransition(to);
            t.canTransitionToSelf = false;
            Configure(t, fade, conditions);
        }

        static void To(AnimatorState from, AnimatorState to, float fade, params (string p, bool v)[] conditions)
        {
            Configure(from.AddTransition(to), fade, conditions);
        }

        static void Configure(AnimatorStateTransition t, float fade, (string p, bool v)[] conditions)
        {
            t.hasExitTime = false;
            t.hasFixedDuration = true;
            t.duration = fade;
            t.interruptionSource = TransitionInterruptionSource.Destination;
            foreach (var (p, v) in conditions)
                t.AddCondition(v ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot, 0f, p);
        }

        static void WirePrefab(AnimatorController controller, Material material, RunnerAnimationProfile profile,
            float getUpLength)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(PrefabPath);
            try
            {
                Transform visual = root.transform.Find("Visual");

                // The capsule blockout stays in the prefab, switched off, so it can come back for debugging.
                foreach (Transform child in visual)
                {
                    if (child.name == "Body" || child.name == "Visor")
                        child.gameObject.SetActive(false);
                }

                Transform existing = visual.Find(ModelChildName);
                if (existing != null)
                    Object.DestroyImmediate(existing.gameObject);

                var model = (GameObject)PrefabUtility.InstantiatePrefab(
                    AssetDatabase.LoadAssetAtPath<GameObject>(RunnerAssetPostprocessor.RunnerModelPath), visual);
                model.name = ModelChildName;
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;
                foreach (Transform t in model.GetComponentsInChildren<Transform>(true))
                    t.gameObject.layer = CosmeticLayer;

                foreach (SkinnedMeshRenderer smr in model.GetComponentsInChildren<SkinnedMeshRenderer>(true))
                {
                    smr.sharedMaterial = material;
                    smr.updateWhenOffscreen = false;
                }

                Animator animator = model.GetComponent<Animator>();
                animator.runtimeAnimatorController = controller;
                animator.applyRootMotion = false;
                animator.updateMode = AnimatorUpdateMode.Normal;
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

                RunnerAnimator driver = visual.GetComponent<RunnerAnimator>();
                if (driver == null)
                    driver = visual.gameObject.AddComponent<RunnerAnimator>();
                var so = new SerializedObject(driver);
                so.FindProperty("profile").objectReferenceValue = profile;
                so.FindProperty("movementProfile").objectReferenceValue =
                    AssetDatabase.LoadAssetAtPath<MovementProfile>(MovementProfilePath);
                so.FindProperty("animator").objectReferenceValue = animator;
                so.FindProperty("getUpClipLength").floatValue = getUpLength;
                so.ApplyModifiedPropertiesWithoutUndo();

                PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
    }
}
