using UnityEngine;

namespace Tumble.Simulation
{
    public struct PlayerSnapshot
    {
        public PlayerId Id;
        public int Tick;
        public MotorState State;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public bool Grounded;
        public bool Qualified;
    }
}
