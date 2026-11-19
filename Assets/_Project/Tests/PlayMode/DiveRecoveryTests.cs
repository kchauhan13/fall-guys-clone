using System.Collections;
using NUnit.Framework;
using Tumble.Simulation;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tumble.Tests
{
    // ADR-0008: the dive ends by itself. The runner slides, then gets back up, with no input.
    public sealed class DiveRecoveryTests
    {
        GameObject floor;
        GameObject runnerObject;
        MotorRig rig;

        MovementProfile profile;

        [SetUp]
        public void SetUp()
        {
            profile = ScriptableObject.CreateInstance<MovementProfile>();
            profile.maxGroundSpeed = 7f;
            profile.groundAcceleration = 18f;
            profile.groundDeceleration = 24f;
            profile.turnResponsiveness = 8f;
            profile.airControlMultiplier = 0.35f;
            profile.slopeLimit = 50f;
            profile.jumpSpeed = 7.5f;
            profile.riseGravityMultiplier = 1.7f;
            profile.lowJumpGravityMultiplier = 4f;
            profile.fallGravityMultiplier = 2.4f;
            profile.coyoteTime = 0.08f;
            profile.jumpBufferTime = 0.1f;
            profile.diveSpeed = 12f;
            profile.diveUpImpulse = 2.5f;
            profile.diveDuration = 0.55f;
            profile.diveCooldown = 0.4f;
            profile.diveRecoverSpeed = 2f;
            profile.divePitch = 75f;
            profile.divePitchTime = 0.12f;
            profile.getUpDuration = 0.3f;

            floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.layer = LayerMask.NameToLayer("Ground");
            floor.transform.position = new Vector3(0f, -0.5f, 0f);
            floor.transform.localScale = new Vector3(60f, 1f, 60f);

            BuildRunner();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(runnerObject);
            Object.DestroyImmediate(floor);
            Object.DestroyImmediate(profile);
        }

        void BuildRunner()
        {
            runnerObject = new GameObject("Runner");
            runnerObject.layer = LayerMask.NameToLayer("Runner");
            runnerObject.transform.position = new Vector3(0f, 0.05f, 0f);

            var capsule = runnerObject.AddComponent<CapsuleCollider>();
            capsule.radius = 0.45f;
            capsule.height = 1.8f;
            capsule.center = new Vector3(0f, 0.9f, 0f);

            var body = runnerObject.AddComponent<Rigidbody>();
            body.mass = 1f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            var motor = runnerObject.AddComponent<PlayerMotor>();
            motor.Initialize(new PlayerId(0), profile);

            rig = runnerObject.AddComponent<MotorRig>();
            rig.motor = motor;
            rig.clock = new RoundClock(Time.fixedDeltaTime);
        }

        [UnityTest]
        public IEnumerator Dive_OnFlatGround_ReturnsToUprightWithoutInput()
        {
            yield return new UnityEngine.WaitForSeconds(0.5f);

            rig.DivePressed = true;
            yield return WaitForState(MotorState.Diving, 1f);

            Assert.That(rig.motor.State, Is.EqualTo(MotorState.Diving), "The dive should start on the edge.");

            yield return WaitForState(MotorState.Upright, 4f);

            Assert.That(rig.motor.State, Is.EqualTo(MotorState.Upright),
                "The dive has to end by itself: slide, then get up, with no input.");
        }

        [UnityTest]
        public IEnumerator Dive_EndsWithTheRunnerStoppedAndUpright()
        {
            yield return new UnityEngine.WaitForSeconds(0.5f);

            rig.DivePressed = true;
            yield return WaitForState(MotorState.Diving, 1f);
            yield return WaitForState(MotorState.Upright, 4f);

            Vector3 velocity = runnerObject.GetComponent<Rigidbody>().velocity;
            float speed = new Vector2(velocity.x, velocity.z).magnitude;
            Assert.That(speed, Is.LessThan(1f), "The runner must not keep running after the dive.");

            Vector3 euler = runnerObject.transform.eulerAngles;
            Assert.That(Mathf.Abs(Mathf.DeltaAngle(euler.x, 0f)), Is.LessThan(5f),
                "The body has to be upright again after the get-up.");
            Assert.That(Mathf.Abs(Mathf.DeltaAngle(euler.z, 0f)), Is.LessThan(5f),
                "The body has to be upright again after the get-up.");
        }

        [UnityTest]
        public IEnumerator Dive_CoversMoreGroundThanARunOverTheDiveWindow()
        {
            yield return new UnityEngine.WaitForSeconds(0.5f);

            rig.Move = Vector2.up;
            yield return new UnityEngine.WaitForSeconds(profile.diveDuration);
            float runDistance = runnerObject.transform.position.z;

            rig.Move = Vector2.zero;
            rig.motor.Teleport(new Vector3(0f, 0.05f, 0f), Quaternion.identity);
            yield return new UnityEngine.WaitForSeconds(0.5f);

            rig.DivePressed = true;
            yield return new UnityEngine.WaitForSeconds(profile.diveDuration);
            float diveDistance = runnerObject.transform.position.z;

            Assert.That(diveDistance, Is.GreaterThan(runDistance + 2f),
                "Over the dive window the dive has to cover much more ground than the run.");
        }

        [UnityTest]
        public IEnumerator Dive_CannotBeSpammedInsideTheCooldown()
        {
            // A long cooldown makes the window unambiguous.
            profile.diveCooldown = 1.5f;

            yield return new UnityEngine.WaitForSeconds(0.5f);

            rig.DivePressed = true;
            yield return WaitForState(MotorState.Diving, 1f);
            yield return WaitForState(MotorState.Upright, 4f);

            // Spam the dive button for a fifth of a second: the cooldown has to hold it off.
            float deadline = Time.time + 0.15f;
            bool dived = false;
            while (Time.time < deadline)
            {
                rig.DivePressed = true;
                yield return null;
                if (rig.motor.State == MotorState.Diving)
                {
                    dived = true;
                    break;
                }
            }

            Assert.That(dived, Is.False, "A second dive inside the cooldown has to be refused.");

            // Once the cooldown is over the dive works again.
            yield return new UnityEngine.WaitForSeconds(1.6f);
            rig.DivePressed = true;
            yield return WaitForState(MotorState.Diving, 1f);
            Assert.That(rig.motor.State, Is.EqualTo(MotorState.Diving),
                "The dive has to be available again after the cooldown.");
        }

        IEnumerator WaitForState(MotorState state, float timeout)
        {
            float deadline = Time.time + timeout;
            while (Time.time < deadline && rig.motor.State != state)
                yield return null;
        }

        // Drives the motor once per physics tick, the way the Gym bootstrap does.
        sealed class MotorRig : MonoBehaviour
        {
            public PlayerMotor motor;
            public RoundClock clock;
            public Vector2 Move;
            public bool DivePressed;
            public float LookYaw;

            void FixedUpdate()
            {
                clock.Advance();
                var command = new PlayerCommand
                {
                    Tick = clock.Tick,
                    Move = Move,
                    LookYaw = LookYaw,
                    DivePressed = DivePressed,
                };
                DivePressed = false;
                motor.Simulate(command, clock, Time.fixedDeltaTime);
            }
        }
    }
}
