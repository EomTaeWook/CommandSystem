namespace CommandSystem.Interface
{
    public interface ICommandProcessor
    {
        void RunCommand(string command);
        string GetModuleName();
    }
}
