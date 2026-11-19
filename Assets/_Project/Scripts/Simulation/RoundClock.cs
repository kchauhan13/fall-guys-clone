namespace Tumble.Simulation
{
    public sealed class RoundClock
    {
        readonly float fixedDeltaTime;

        public RoundClock(float fixedDeltaTime)
        {
            this.fixedDeltaTime = fixedDeltaTime;
        }

        public int Tick { get; private set; }
        public float Time => Tick * fixedDeltaTime;

        public void Advance()
        {
            Tick++;
        }
    }
}
