using Dignus.Actor.Core;

namespace Dignus.Commands.Interfaces
{
    public interface IPathCommand
    {
        Task InvokeAsync(string[] args, string currentPath, IActorRef sender, CancellationToken cancellationToken);
        string Print();
    }
}
