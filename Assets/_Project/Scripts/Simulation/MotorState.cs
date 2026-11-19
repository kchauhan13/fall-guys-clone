namespace Tumble.Simulation
{
    // ADR-0008: the runner is never knocked down. GettingUp is the post-dive recovery only.
    public enum MotorState : byte
    {
        Upright = 0,
        Diving = 1,
        GettingUp = 2,
        Grabbing = 3,
    }
}
