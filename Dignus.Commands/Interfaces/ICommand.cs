using Dignus.Actor.Core;

namespace Dignus.Commands.Interfaces
{
    public interface ICommand : IPathCommand
    {
        Task InvokeAsync(string[] args, IActorRef sender, CancellationToken cancellationToken);

        Task IPathCommand.InvokeAsync(string[] args, string currentPath, IActorRef sender, CancellationToken cancellationToken) 
        {
            return InvokeAsync(args, sender, cancellationToken);
        } 
    }
}
