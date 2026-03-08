using Dignus.Actor.Core.Actors;
using Dignus.Commands.Interfaces;

namespace Dignus.Commands.Pipeline
{
    public readonly struct CommandPipelineContext
    {
        public ICommand Command { get; init; }
        public IActorRef SenderActorRef { get; init; }
        public string[] CommandArguments { get; init; }

        public CancellationToken CancellationToken { get; init; }
    }
}
