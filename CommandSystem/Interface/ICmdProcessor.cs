namespace CommandSystem.Interface
{
    public interface ICmdProcessor
    {
        Task InvokeAsync(string[] args, CancellationToken cancellationToken);
        string Print();
    }
}
