namespace Tumble.Simulation
{
    public interface IMotorEvents
    {
        void Jumped(PlayerId id);
        void Dived(PlayerId id);
        void Landed(PlayerId id, float impactSpeed);
        void GotUp(PlayerId id);
        void Grabbed(PlayerId id);
        void GrabBroke(PlayerId id);
    }
}
