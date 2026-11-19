using UnityEngine;

namespace Tumble.Presentation
{
    // Values live on the asset. The Start column in docs/game-feel.md ("Animation") is the reference.
    // See docs/specs/SPEC-012-runner-animation.md.
    [CreateAssetMenu(menuName = "Tumble/Runner Animation Profile", fileName = "RunnerAnimationProfile")]
    public sealed class RunnerAnimationProfile : ScriptableObject
    {
        [Header("Locomotion")]
        [Tooltip("Ground speed (m/s) at which the Walk clip's feet stop sliding. Calibrated per clip.")]
        public float walkCycleSpeed;
        [Tooltip("Ground speed (m/s) at which the Run clip's feet stop sliding. Calibrated per clip.")]
        public float runCycleSpeed;
        [Tooltip("Seconds of damping on the locomotion blend.")]
        public float locomotionBlendDamp;

        [Header("Airborne")]
        [Tooltip("Vertical speed (m/s) at which the Rise or Fall pose is fully weighted.")]
        public float airBlendSpeedBand;
        [Tooltip("Seconds without ground before a non-jump fall shows the air pose.")]
        public float airPoseDelay;

        [Header("Transitions")]
        [Tooltip("Fixed crossfade, in seconds, for every Animator transition.")]
        public float stateCrossfade;

        [Header("Turn lean")]
        [Tooltip("Degrees")]
        public float maxTurnLean;
        [Tooltip("Lateral acceleration (m/s^2) at which the lean reaches its maximum.")]
        public float leanFullLateralAccel;
        public float turnLeanResponsiveness;

        [Header("Landing squash")]
        [Tooltip("Fraction of height lost at the strongest squash.")]
        public float landSquash;
        [Tooltip("Impact speed (m/s) that produces the full squash.")]
        public float landSquashFullImpact;
        public float landSquashDuration;
    }
}
