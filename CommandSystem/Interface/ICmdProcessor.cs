namespace CommandSystem.Interface
{
    public interface ICmdProcessor
    {
        Task InvokeAsync(string[] args);
        string Print();
    }
}
