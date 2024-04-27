namespace CommandSystem.Interfaces
{
    public interface ICommandProcessor
    {
        void RunCommand(string command);
        string GetModuleName();
    }
}
