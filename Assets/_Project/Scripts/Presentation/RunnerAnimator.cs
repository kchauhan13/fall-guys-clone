using Tumble.Simulation;
using UnityEngine;

namespace Tumble.Presentation
{
    // Drives the runner's Animator and two procedural layers (turn lean, landing squash) from the motor.
    // Read-only toward simulation: never writes velocity, rotation, state, or constraints (ADR-0001).
    // Lives on the Visual child; the skinned model with its Animator is a child of this object.
    // See docs/specs/SPEC-012-runner-animation.md.
    public sealed class RunnerAnimator : MonoBehaviour, IMotorEvents
    {
        public static readonly int SpeedParam = Animator.StringToHash("Speed");
        public static readonly int LocomotionScaleParam = Animator.StringToHash("LocomotionScale");
        public static readonly int VerticalSpeedParam = Animator.StringToHash("VerticalSpeed");
        public static readonly int AirborneParam = Animator.StringToHash("Airborne");
        public static readonly int DivingParam = Animator.StringToHash("Diving");
        public static readonly int GettingUpParam = Animator.StringToHash("GettingUp");
        public static readonly int GetUpScaleParam = Animator.StringToHash("GetUpScale");

        // Blend coordinates used by AC_Runner. The profile maps real speeds onto them at runtime,
        // so recalibrating a clip never requires rebuilding the controller.
        public const float IdleCoord = 0f;
        public const float WalkCoord = 1f;
        public const float RunCoord = 2f;

        const float MinSpeed = 0.05f;

        [SerializeField] RunnerAnimationProfile profile;
        [Tooltip("Same asset GymBootstrap passes to the motor. Used only for getUpDuration.")]
        [SerializeField] MovementProfile movementProfile;
        [SerializeField] Animator animator;
        [Tooltip("Length in seconds of the GetUp clip at 1x. Filled in by the animator builder.")]
        [SerializeField] float getUpClipLength = 1f;

        PlayerMotor motor;
        Rigidbody body;

        bool jumpedThisAir;
        float ungroundedTime;
        float lastYaw;
        float lean;
        float squashAmount;
        float squashTime = float.PositiveInfinity;

        void Awake()
        {
            motor = GetComponentInParent<PlayerMotor>();
            body = GetComponentInParent<Rigidbody>();
            if (animator == null)
                animator = GetComponentInChildren<Animator>();

            animator.applyRootMotion = false;
            animator.updateMode = AnimatorUpdateMode.Normal;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }

        void OnEnable()
        {
            motor.AddListener(this);
            lastYaw = motor.transform.eulerAngles.y;
        }

        void OnDisable() => motor.RemoveListener(this);

        void Update()
        {
            float dt = Time.deltaTime;
            if (dt <= 0f)
                return;

            MotorState state = motor.State;
            bool upright = state == MotorState.Upright;
            Vector3 velocity = body.velocity;
            float horizontalSpeed = new Vector2(velocity.x, velocity.z).magnitude;

            // Airborne: immediate on a jump, delayed for a walk-off so probe flicker never pops the pose.
            if (motor.Grounded)
            {
                ungroundedTime = 0f;
                jumpedThisAir = false;
            }
            else
            {
                ungroundedTime += dt;
            }
            bool airborne = upright && !motor.Grounded && (jumpedThisAir || ungroundedTime >= profile.airPoseDelay);

            animator.SetFloat(SpeedParam, LocomotionCoord(horizontalSpeed), profile.locomotionBlendDamp, dt);
            animator.SetFloat(LocomotionScaleParam,
                horizontalSpeed > profile.runCycleSpeed && profile.runCycleSpeed > 0f
                    ? horizontalSpeed / profile.runCycleSpeed
                    : 1f);
            animator.SetFloat(VerticalSpeedParam,
                profile.airBlendSpeedBand > 0f ? Mathf.Clamp(velocity.y / profile.airBlendSpeedBand, -1f, 1f) : 0f);
            animator.SetBool(AirborneParam, airborne);
            animator.SetBool(DivingParam, state == MotorState.Diving);
            animator.SetBool(GettingUpParam, state == MotorState.GettingUp);
            animator.SetFloat(GetUpScaleParam,
                movementProfile != null && movementProfile.getUpDuration > 0f
                    ? getUpClipLength / movementProfile.getUpDuration
                    : 1f);

            UpdateLean(upright, horizontalSpeed, dt);
            UpdateSquash(dt);
            ApplyVisualTransform();
        }

        float LocomotionCoord(float speed)
        {
            if (speed < MinSpeed)
                return IdleCoord;
            float walk = Mathf.Max(profile.walkCycleSpeed, MinSpeed);
            if (speed <= walk)
                return Mathf.Lerp(IdleCoord, WalkCoord, speed / walk);
            float run = Mathf.Max(profile.runCycleSpeed, walk + MinSpeed);
            return Mathf.Lerp(WalkCoord, RunCoord, (speed - walk) / (run - walk));
        }

        void UpdateLean(bool upright, float horizontalSpeed, float dt)
        {
            float yaw = motor.transform.eulerAngles.y;
            float yawRate = Mathf.DeltaAngle(lastYaw, yaw) * Mathf.Deg2Rad / dt;
            lastYaw = yaw;

            float target = 0f;
            if (upright && motor.Grounded && profile.leanFullLateralAccel > 0f)
            {
                // Centripetal acceleration of the turn; positive yaw rate is a right turn.
                float lateral = yawRate * horizontalSpeed;
                target = -profile.maxTurnLean * Mathf.Clamp(lateral / profile.leanFullLateralAccel, -1f, 1f);
            }
            lean = Mathf.Lerp(lean, target, MotorMath.ExponentialBlend(profile.turnLeanResponsiveness, dt));
        }

        void UpdateSquash(float dt)
        {
            squashTime += dt;
        }

        void ApplyVisualTransform()
        {
            float squash = 0f;
            if (profile.landSquashDuration > 0f && squashTime < profile.landSquashDuration)
            {
                float t = squashTime / profile.landSquashDuration;
                squash = squashAmount * (1f - t) * (1f - t);
            }

            // Volume-preserving squash from the feet pivot.
            float y = 1f - squash;
            float xz = 1f / Mathf.Sqrt(Mathf.Max(y, 0.01f));
            transform.localScale = new Vector3(xz, y, xz);
            transform.localRotation = Quaternion.Euler(0f, 0f, lean);
        }

        // IMotorEvents. Called from the motor's fixed tick; only latches presentation state here.
        public void Jumped(PlayerId id) => jumpedThisAir = true;

        public void Landed(PlayerId id, float impactSpeed)
        {
            if (profile.landSquashFullImpact <= 0f)
                return;
            squashAmount = profile.landSquash * Mathf.Clamp01(impactSpeed / profile.landSquashFullImpact);
            squashTime = 0f;
        }

        public void Dived(PlayerId id) { }
        public void GotUp(PlayerId id) { }
        public void Grabbed(PlayerId id) { }
        public void GrabBroke(PlayerId id) { }
    }
}
