namespace CommandSystem.Interfaces
{
    public interface ICommandAction
    {
        Task InvokeAsync(string[] args, CancellationToken cancellationToken);
        string Print();
    }
}
