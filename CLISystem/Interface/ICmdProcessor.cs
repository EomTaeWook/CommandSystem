namespace CLISystem.Interface
{
    public interface ICmdProcessor
    {
        Task InvokeAsync(string[] args);
        string Print();
    }
}
