using Dignus.Actor.Core.Actors;

namespace Dignus.Commands.Interfaces
{
    public interface ICommand
    {
        Task InvokeAsync(string[] args, IActorRef sender, CancellationToken cancellationToken);
        string Print();
    }
}
