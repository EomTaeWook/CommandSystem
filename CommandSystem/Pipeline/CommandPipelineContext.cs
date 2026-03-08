using CommandSystem.Interfaces;
using Dignus.Actor.Core.Actors;

namespace CommandSystem.Pipeline
{
    public readonly struct CommandPipelineContext
    {
        public ICommand Command { get; init; }
        public IActorRef SenderActorRef { get; init; }
        public string[] CommandArguments { get; init; }

        public CancellationToken CancellationToken { get; init; }
    }
}
