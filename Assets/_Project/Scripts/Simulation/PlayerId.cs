using System;

namespace Tumble.Simulation
{
    public readonly struct PlayerId : IEquatable<PlayerId>
    {
        public readonly int Value;

        public PlayerId(int value)
        {
            Value = value;
        }

        public bool Equals(PlayerId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is PlayerId other && Equals(other);
        public override int GetHashCode() => Value;
        public override string ToString() => $"Player {Value}";

        public static bool operator ==(PlayerId a, PlayerId b) => a.Value == b.Value;
        public static bool operator !=(PlayerId a, PlayerId b) => a.Value != b.Value;
    }
}
