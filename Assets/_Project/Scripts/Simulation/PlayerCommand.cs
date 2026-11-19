using UnityEngine;

namespace Tumble.Simulation
{
    public struct PlayerCommand
    {
        public int Tick;
        public Vector2 Move;
        public float LookYaw;
        public bool JumpPressed;
        public bool JumpHeld;
        public bool DivePressed;
        public bool GrabHeld;
    }
}
