namespace Tumble.Simulation
{
    // Coyote time and jump buffer. Plain C# so the rules are testable without physics.
    public sealed class JumpGate
    {
        float lastGroundedTime = float.NegativeInfinity;
        float bufferedUntil = float.NegativeInfinity;

        public bool JumpedSinceGrounded { get; private set; }

        public bool Step(float time, bool grounded, bool jumpPressed, float coyoteTime, float jumpBufferTime)
        {
            if (grounded)
            {
                lastGroundedTime = time;
                JumpedSinceGrounded = false;
            }

            if (jumpPressed)
                bufferedUntil = time + jumpBufferTime;

            bool canJump = !JumpedSinceGrounded && (grounded || time - lastGroundedTime <= coyoteTime);
            if (!canJump || time > bufferedUntil)
                return false;

            JumpedSinceGrounded = true;
            bufferedUntil = float.NegativeInfinity;
            return true;
        }

        public void Reset()
        {
            lastGroundedTime = float.NegativeInfinity;
            bufferedUntil = float.NegativeInfinity;
            JumpedSinceGrounded = false;
        }
    }
}
