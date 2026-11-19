using NUnit.Framework;
using Tumble.Simulation;

namespace Tumble.Tests
{
    public sealed class JumpGateTests
    {
        const float Coyote = 0.08f;
        const float Buffer = 0.10f;

        JumpGate gate;

        [SetUp]
        public void SetUp()
        {
            gate = new JumpGate();
        }

        [Test]
        public void GroundedPress_Jumps()
        {
            Assert.That(gate.Step(0f, true, true, Coyote, Buffer), Is.True);
        }

        [Test]
        public void PressInsideCoyoteWindow_Jumps()
        {
            gate.Step(0f, true, false, Coyote, Buffer);
            gate.Step(0.02f, false, false, Coyote, Buffer);

            Assert.That(gate.Step(0.06f, false, true, Coyote, Buffer), Is.True);
        }

        [Test]
        public void PressAfterCoyoteWindow_DoesNotJump()
        {
            gate.Step(0f, true, false, Coyote, Buffer);

            Assert.That(gate.Step(0.12f, false, true, Coyote, Buffer), Is.False);
        }

        [Test]
        public void BufferedPress_JumpsOnLandingInsideWindow()
        {
            Assert.That(gate.Step(1.00f, false, true, Coyote, Buffer), Is.False);
            Assert.That(gate.Step(1.08f, true, false, Coyote, Buffer), Is.True);
        }

        [Test]
        public void BufferedPress_ExpiresBeforeLanding()
        {
            gate.Step(1.00f, false, true, Coyote, Buffer);

            Assert.That(gate.Step(1.14f, true, false, Coyote, Buffer), Is.False);
        }

        [Test]
        public void SecondPressWithoutLanding_DoesNotJump()
        {
            gate.Step(0f, true, true, Coyote, Buffer);

            Assert.That(gate.Step(0.04f, false, true, Coyote, Buffer), Is.False);
        }

        [Test]
        public void BufferIsSpentByJump()
        {
            gate.Step(0f, true, true, Coyote, Buffer);

            Assert.That(gate.Step(0.06f, true, false, Coyote, Buffer), Is.False);
        }
    }
}
