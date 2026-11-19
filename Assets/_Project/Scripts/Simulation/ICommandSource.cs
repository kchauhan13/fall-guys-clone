namespace Tumble.Simulation
{
    public interface ICommandSource
    {
        PlayerCommand CreateCommand(int tick);
    }
}
