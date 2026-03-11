using Dignus.Actor.Core;
using Dignus.Commands.Interfaces;

namespace Dignus.Commands.Pipeline
{
    public readonly struct CommandPipelineContext
    {
        public IPathCommand Command { get; init; }
        public IActorRef SenderActorRef { get; init; }
        public string CurrentPath { get; init; }
        public string[] CommandArguments { get; init; }

        public CancellationToken CancellationToken { get; init; }
    }
}
