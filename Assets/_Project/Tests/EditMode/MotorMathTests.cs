using System.Linq;
using NUnit.Framework;
using Tumble.Simulation;
using UnityEngine;

namespace Tumble.Tests
{
    public sealed class MotorMathTests
    {
        const float Tolerance = 0.0001f;

        [Test]
        public void WishDirection_ForwardAtYaw90_PointsAlongWorldRight()
        {
            Vector3 wish = MotorMath.WishDirection(Vector2.up, 90f);

            Assert.That(Vector3.Distance(wish, Vector3.right), Is.LessThan(Tolerance));
        }

        [Test]
        public void WishDirection_DiagonalStick_IsClampedToUnitLength()
        {
            Vector3 wish = MotorMath.WishDirection(new Vector2(1f, 1f), 0f);

            Assert.That(wish.magnitude, Is.EqualTo(1f).Within(Tolerance));
        }

        [Test]
        public void StepTowardWish_ReleasedStick_UsesDeceleration()
        {
            Vector3 next = MotorMath.StepTowardWish(new Vector3(7f, 0f, 0f), Vector3.zero, 18f, 24f, 0.02f);

            Assert.That(next.x, Is.EqualTo(7f - 24f * 0.02f).Within(Tolerance));
        }

        [Test]
        public void StepTowardWish_SpeedingUp_UsesAcceleration()
        {
            Vector3 next = MotorMath.StepTowardWish(Vector3.zero, new Vector3(0f, 0f, 7f), 18f, 24f, 0.02f);

            Assert.That(next.z, Is.EqualTo(18f * 0.02f).Within(Tolerance));
        }

        [Test]
        public void ExtraGravity_RisingWithoutJump_AddsNothing()
        {
            Assert.That(MotorMath.ExtraGravity(2f, false, true, 9.81f, 1.7f, 4f, 2.4f), Is.EqualTo(0f));
        }

        [Test]
        public void ExtraGravity_RisingAndHeld_UsesRiseMultiplier()
        {
            float extra = MotorMath.ExtraGravity(2f, true, true, 9.81f, 1.7f, 4f, 2.4f);

            Assert.That(extra, Is.EqualTo((1.7f - 1f) * 9.81f).Within(Tolerance));
        }

        [Test]
        public void ExtraGravity_RisingAndReleased_UsesLowJumpMultiplier()
        {
            float extra = MotorMath.ExtraGravity(2f, true, false, 9.81f, 1.7f, 4f, 2.4f);

            Assert.That(extra, Is.EqualTo((4f - 1f) * 9.81f).Within(Tolerance));
        }

        [Test]
        public void ExtraGravity_Falling_IgnoresHoldAndUsesFallMultiplier()
        {
            float held = MotorMath.ExtraGravity(-2f, true, true, 9.81f, 1.7f, 4f, 2.4f);
            float released = MotorMath.ExtraGravity(-2f, true, false, 9.81f, 1.7f, 4f, 2.4f);

            Assert.That(held, Is.EqualTo((2.4f - 1f) * 9.81f).Within(Tolerance));
            Assert.That(released, Is.EqualTo(held).Within(Tolerance));
        }

        [Test]
        public void SimulationAssembly_DoesNotReferenceInputSystem()
        {
            var references = typeof(PlayerMotor).Assembly.GetReferencedAssemblies().Select(a => a.Name);

            Assert.That(references, Has.None.StartsWith("Unity.InputSystem"));
        }
    }
}
