using UnityEngine;

namespace Tumble.Simulation
{
    public static class MotorMath
    {
        public static Vector3 WishDirection(Vector2 move, float lookYaw)
        {
            Vector2 stick = Vector2.ClampMagnitude(move, 1f);
            return Quaternion.Euler(0f, lookYaw, 0f) * new Vector3(stick.x, 0f, stick.y);
        }

        public static Vector3 StepTowardWish(Vector3 current, Vector3 wish, float acceleration, float deceleration, float dt)
        {
            float rate = wish.magnitude < current.magnitude ? deceleration : acceleration;
            return Vector3.MoveTowards(current, wish, rate * dt);
        }

        // Variable jump height. While rising from a jump: holding the button uses the lighter rise
        // gravity (full height), releasing early uses the heavier low-jump gravity (cut short). A rise
        // that did not come from a jump, and all falling, ignore the hold.
        public static float ExtraGravity(float verticalSpeed, bool jumpedSinceGrounded, bool jumpHeld,
            float gravity, float riseMultiplier, float lowJumpMultiplier, float fallMultiplier)
        {
            if (verticalSpeed > 0f)
            {
                if (!jumpedSinceGrounded)
                    return 0f;
                float multiplier = jumpHeld ? riseMultiplier : lowJumpMultiplier;
                return (multiplier - 1f) * gravity;
            }
            return (fallMultiplier - 1f) * gravity;
        }

        public static float ExponentialBlend(float responsiveness, float dt)
        {
            return 1f - Mathf.Exp(-responsiveness * dt);
        }
    }
}
