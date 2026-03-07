using Dignus.Actor.Core.Actors;

namespace CommandSystem.Interfaces
{
    public interface ICommand
    {
        Task InvokeAsync(string[] args, IActorRef sender, CancellationToken cancellationToken);
        string Print();
    }
}
