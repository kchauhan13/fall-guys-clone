using UnityEngine;

namespace Tumble.Simulation
{
    // Values live on the asset. The Start column in docs/game-feel.md is the reference.
    [CreateAssetMenu(menuName = "Tumble/Movement Profile", fileName = "MovementProfile")]
    public sealed class MovementProfile : ScriptableObject
    {
        [Header("Move")]
        public float maxGroundSpeed;
        public float groundAcceleration;
        public float groundDeceleration;
        public float turnResponsiveness;
        public float airControlMultiplier;
        [Tooltip("Degrees")]
        public float slopeLimit;

        [Header("Jump")]
        public float jumpSpeed;
        public float riseGravityMultiplier;
        public float lowJumpGravityMultiplier;
        public float fallGravityMultiplier;
        public float coyoteTime;
        public float jumpBufferTime;

        [Header("Dive")]
        public float diveSpeed;
        public float diveUpImpulse;
        public float diveDuration;
        public float diveCooldown;
        [Tooltip("Horizontal speed under which the dive slide ends and the runner gets back up.")]
        public float diveRecoverSpeed;
        [Tooltip("Degrees of forward pitch for the belly dive. The motor drives this, not physics.")]
        public float divePitch;
        [Tooltip("Seconds for the body to reach the full dive pitch.")]
        public float divePitchTime;

        [Header("Get up")]
        [Tooltip("Seconds for the motor to slerp the body back upright after a dive.")]
        public float getUpDuration;

        [Header("Grab")]
        public float grabRange;
        public float grabBreakDistance;
    }
}
