using System.Collections.Generic;
using UnityEngine;

namespace Tumble.Simulation
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        const float GroundProbeLift = 0.05f;
        const float GroundProbeReach = 0.12f;
        const float GroundProbeRadiusScale = 0.95f;
        // Keeps the probe from re-grounding the runner on the tick after takeoff.
        const float JumpGroundLockout = 0.1f;
        const float MinWishSqr = 0.0001f;
        // The dive only lets the stick nudge facing, so steering is a fraction of ground turning.
        const float DiveSteerScale = 0.25f;
        const RigidbodyConstraints UprightConstraints =
            RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        const RigidbodyConstraints LooseConstraints = RigidbodyConstraints.None;

        [SerializeField] CapsuleCollider capsule;
        [SerializeField] LayerMask groundMask;

        readonly JumpGate jumpGate = new JumpGate();
        readonly List<IMotorEvents> listeners = new List<IMotorEvents>();

        Rigidbody body;
        MovementProfile profile;
        float facingYaw;
        float groundLockoutUntil = float.NegativeInfinity;
        Vector3 groundNormal = Vector3.up;
        Vector3 lastWrittenVelocity;

        bool jumpCutLatched;
        float diveStartTime;
        float diveEndTime;
        float diveCooldownUntil = float.NegativeInfinity;
        float getUpStartTime;
        Quaternion getUpFromRotation = Quaternion.identity;

        public PlayerId Id { get; private set; }
        public MotorState State { get; private set; } = MotorState.Upright;
        public bool Grounded { get; private set; }
        public CapsuleCollider Capsule => capsule;
        public Vector3 Position => body.position;

        void Awake()
        {
            body = GetComponent<Rigidbody>();
            if (capsule == null)
                capsule = GetComponentInChildren<CapsuleCollider>();
            if (groundMask.value == 0)
                groundMask = ContractGroundMask();
            body.constraints = UprightConstraints;
            facingYaw = transform.eulerAngles.y;
        }

        // The contract fixes the ground mask: Ground and Obstacle tops count as floor.
        static LayerMask ContractGroundMask()
        {
            int mask = 0;
            int ground = LayerMask.NameToLayer("Ground");
            int obstacle = LayerMask.NameToLayer("Obstacle");
            if (ground >= 0) mask |= 1 << ground;
            if (obstacle >= 0) mask |= 1 << obstacle;
            return mask;
        }

        public void Initialize(PlayerId id, MovementProfile movementProfile)
        {
            Id = id;
            profile = movementProfile;
        }

        public void AddListener(IMotorEvents listener) => listeners.Add(listener);
        public void RemoveListener(IMotorEvents listener) => listeners.Remove(listener);

        public void Simulate(PlayerCommand command, RoundClock clock, float dt)
        {
            if (body == null)
                return; // Play mode can tear the body down while a tick is still in flight.

            bool wasGrounded = Grounded;
            Grounded = clock.Time >= groundLockoutUntil && ProbeGround(out groundNormal);
            bool justLanded = Grounded && !wasGrounded;
            if (justLanded)
            {
                float impactSpeed = Mathf.Max(0f, -lastWrittenVelocity.y);
                for (int i = 0; i < listeners.Count; i++)
                    listeners[i].Landed(Id, impactSpeed);
            }

            switch (State)
            {
                case MotorState.Upright:
                    SimulateUpright(command, clock, dt);
                    break;
                case MotorState.Diving:
                    SimulateDiving(command, clock, dt);
                    break;
                case MotorState.GettingUp:
                    SimulateGettingUp(clock, dt);
                    break;
                case MotorState.Grabbing:
                    break;
            }
        }

        public void Teleport(Vector3 position, Quaternion rotation)
        {
            facingYaw = rotation.eulerAngles.y;
            Quaternion upright = Quaternion.Euler(0f, facingYaw, 0f);

            body.position = position;
            body.rotation = upright;
            transform.SetPositionAndRotation(position, upright);
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            body.constraints = UprightConstraints;

            State = MotorState.Upright;
            Grounded = false;
            jumpCutLatched = false;
            groundLockoutUntil = float.NegativeInfinity;
            diveCooldownUntil = float.NegativeInfinity;
            lastWrittenVelocity = Vector3.zero;
            jumpGate.Reset();
        }

        public PlayerSnapshot CaptureSnapshot(int tick)
        {
            return new PlayerSnapshot
            {
                Id = Id,
                Tick = tick,
                State = State,
                Position = body.position,
                Rotation = body.rotation,
                Velocity = body.velocity,
                Grounded = Grounded,
            };
        }

        void SimulateUpright(PlayerCommand command, RoundClock clock, float dt)
        {
            if (command.DivePressed && clock.Time >= diveCooldownUntil)
            {
                EnterDive(command, clock);
                return;
            }

            Vector3 wishDirection = MotorMath.WishDirection(command.Move, command.LookYaw);
            Vector3 wish = wishDirection * profile.maxGroundSpeed;
            Vector3 velocity = body.velocity;

            if (Grounded)
            {
                Vector3 alongGround = Vector3.ProjectOnPlane(velocity, groundNormal);
                Vector3 wishOnGround = Vector3.ProjectOnPlane(wish, groundNormal).normalized * wish.magnitude;
                velocity = MotorMath.StepTowardWish(alongGround, wishOnGround,
                    profile.groundAcceleration, profile.groundDeceleration, dt);
            }
            else if (wish.sqrMagnitude > MinWishSqr)
            {
                Vector3 horizontal = new Vector3(velocity.x, 0f, velocity.z);
                float airAcceleration = profile.groundAcceleration * profile.airControlMultiplier;
                horizontal = Vector3.MoveTowards(horizontal, wish, airAcceleration * dt);
                velocity = new Vector3(horizontal.x, velocity.y, horizontal.z);
            }

            if (jumpGate.Step(clock.Time, Grounded, command.JumpPressed, profile.coyoteTime, profile.jumpBufferTime))
            {
                velocity.y = profile.jumpSpeed;
                Grounded = false;
                jumpCutLatched = false;
                groundLockoutUntil = clock.Time + JumpGroundLockout;
                for (int i = 0; i < listeners.Count; i++)
                    listeners[i].Jumped(Id);
            }

            if (!Grounded)
            {
                // Once the button is released during the rise, the cut sticks for the rest of the
                // ascent so a re-press mid-air cannot restore the floaty rise.
                if (jumpGate.JumpedSinceGrounded && velocity.y > 0f && !command.JumpHeld)
                    jumpCutLatched = true;
                bool jumpHeld = command.JumpHeld && !jumpCutLatched;
                float extraGravity = MotorMath.ExtraGravity(velocity.y, jumpGate.JumpedSinceGrounded, jumpHeld,
                    -Physics.gravity.y, profile.riseGravityMultiplier, profile.lowJumpGravityMultiplier,
                    profile.fallGravityMultiplier);
                velocity.y -= extraGravity * dt;
            }

            if (wish.sqrMagnitude > MinWishSqr)
            {
                float targetYaw = Mathf.Atan2(wishDirection.x, wishDirection.z) * Mathf.Rad2Deg;
                facingYaw = Mathf.LerpAngle(facingYaw, targetYaw,
                    MotorMath.ExponentialBlend(profile.turnResponsiveness, dt));
            }

            body.MoveRotation(Quaternion.Euler(0f, facingYaw, 0f));
            body.angularVelocity = Vector3.zero;
            body.velocity = velocity;
            lastWrittenVelocity = velocity;
        }

        void EnterDive(PlayerCommand command, RoundClock clock)
        {
            State = MotorState.Diving;
            // The dive needs the pitch unlocked, because the motor drives the belly-dive pose itself.
            body.constraints = LooseConstraints;
            facingYaw = command.LookYaw;

            Vector3 flatForward = Quaternion.Euler(0f, facingYaw, 0f) * Vector3.forward;
            Vector3 velocity = flatForward * profile.diveSpeed;
            velocity.y = Mathf.Max(body.velocity.y, profile.diveUpImpulse);
            body.velocity = velocity;
            lastWrittenVelocity = velocity;

            diveStartTime = clock.Time;
            diveEndTime = clock.Time + profile.diveDuration;
            jumpGate.Reset();

            for (int i = 0; i < listeners.Count; i++)
                listeners[i].Dived(Id);
        }

        void SimulateDiving(PlayerCommand command, RoundClock clock, float dt)
        {
            // Stick nudges facing only.
            Vector3 wishDirection = MotorMath.WishDirection(command.Move, command.LookYaw);
            if (wishDirection.sqrMagnitude > MinWishSqr)
            {
                float targetYaw = Mathf.Atan2(wishDirection.x, wishDirection.z) * Mathf.Rad2Deg;
                facingYaw = Mathf.LerpAngle(facingYaw, targetYaw,
                    MotorMath.ExponentialBlend(profile.turnResponsiveness * DiveSteerScale, dt));
            }

            if (clock.Time < diveEndTime)
            {
                // The burst: hold the dive speed along the flat facing. Vertical stays with gravity.
                Vector3 flat = Quaternion.Euler(0f, facingYaw, 0f) * Vector3.forward * profile.diveSpeed;
                Vector3 velocity = new Vector3(flat.x, body.velocity.y, flat.z);
                body.velocity = velocity;
                lastWrittenVelocity = velocity;
            }
            else if (Grounded)
            {
                // The slide: bleed the dive speed with the ground deceleration.
                Vector3 flatVel = new Vector3(body.velocity.x, 0f, body.velocity.z);
                flatVel = Vector3.MoveTowards(flatVel, Vector3.zero, profile.groundDeceleration * dt);
                Vector3 slide = new Vector3(flatVel.x, body.velocity.y, flatVel.z);
                body.velocity = slide;
                lastWrittenVelocity = slide;

                if (flatVel.magnitude <= profile.diveRecoverSpeed)
                {
                    // Slow enough: the runner picks themselves up. No input, no delay.
                    SetDiveCooldown(clock);
                    EnterGettingUp(clock);
                    return;
                }
            }
            else
            {
                // Airborne after the burst: keep the momentum and wait for the landing.
                lastWrittenVelocity = body.velocity;
            }

            ApplyDivePose(clock);
        }

        // The belly dive is authored, not a physics flop (ADR-0008): the motor drives the pitch and
        // kills the angular velocity so the body cannot tumble under friction.
        void ApplyDivePose(RoundClock clock)
        {
            float pitch = DivePitch(clock);
            Quaternion target = Quaternion.Euler(pitch, facingYaw, 0f);
            body.MoveRotation(target);
            body.angularVelocity = Vector3.zero;
        }

        float DivePitch(RoundClock clock)
        {
            if (profile.divePitch <= 0f)
                return 0f;
            if (profile.divePitchTime <= 0f)
                return profile.divePitch;

            float t = Mathf.Clamp01((clock.Time - diveStartTime) / profile.divePitchTime);
            float eased = 1f - (1f - t) * (1f - t); // ease out: the pitch snaps in, then settles.
            return profile.divePitch * eased;
        }

        void EnterGettingUp(RoundClock clock)
        {
            State = MotorState.GettingUp;
            getUpStartTime = clock.Time;
            getUpFromRotation = body.rotation;
            body.constraints = LooseConstraints;
            body.angularVelocity = Vector3.zero;
            jumpGate.Reset();
        }

        void SimulateGettingUp(RoundClock clock, float dt)
        {
            float a = profile.getUpDuration <= 0f
                ? 1f
                : Mathf.Clamp01((clock.Time - getUpStartTime) / profile.getUpDuration);

            Quaternion target = Quaternion.Euler(0f, facingYaw, 0f);
            body.MoveRotation(Quaternion.Slerp(getUpFromRotation, target, a));
            body.angularVelocity = Vector3.zero;

            Vector3 flatVel = new Vector3(body.velocity.x, 0f, body.velocity.z);
            flatVel = Vector3.MoveTowards(flatVel, Vector3.zero, profile.groundDeceleration * dt);
            Vector3 settled = new Vector3(flatVel.x, body.velocity.y, flatVel.z);
            body.velocity = settled;
            lastWrittenVelocity = settled;

            if (a < 1f)
                return;

            ExitToUpright();
            for (int i = 0; i < listeners.Count; i++)
                listeners[i].GotUp(Id);
        }

        void ExitToUpright()
        {
            State = MotorState.Upright;
            body.constraints = UprightConstraints;
            body.MoveRotation(Quaternion.Euler(0f, facingYaw, 0f));
            body.angularVelocity = Vector3.zero;
            jumpGate.Reset();
        }

        void SetDiveCooldown(RoundClock clock)
        {
            diveCooldownUntil = clock.Time + profile.diveCooldown;
        }

        // The capsule is a child of the rigidbody root, and the root swings when the body pitches.
        // Probing from the root therefore misses the floor exactly when the runner is lying down,
        // which used to leave the dive holding its speed forever. Probe the collider instead.
        bool ProbeGround(out Vector3 normal)
        {
            normal = Vector3.up;

            Bounds bounds = capsule.bounds;
            float radius = capsule.radius * GroundProbeRadiusScale;
            Vector3 origin = new Vector3(bounds.center.x, bounds.min.y + radius + GroundProbeLift, bounds.center.z);
            float distance = radius + GroundProbeLift + GroundProbeReach;

            if (Physics.SphereCast(origin, radius, Vector3.down, out RaycastHit hit, distance,
                    groundMask, QueryTriggerInteraction.Ignore))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) > profile.slopeLimit)
                    return false;
                normal = hit.normal;
                return true;
            }

            // A deep overlap (a hard landing) starts the cast inside the floor, and a cast that
            // begins inside a collider reports nothing. A ray dropped from above still sees it.
            Vector3 rayTop = origin + Vector3.up * GroundProbeReach;
            if (Physics.Raycast(rayTop, Vector3.down, out RaycastHit rayHit,
                    GroundProbeReach + distance, groundMask, QueryTriggerInteraction.Ignore))
            {
                if (Vector3.Angle(rayHit.normal, Vector3.up) > profile.slopeLimit)
                    return false;
                normal = rayHit.normal;
                return true;
            }

            return false;
        }
    }
}
