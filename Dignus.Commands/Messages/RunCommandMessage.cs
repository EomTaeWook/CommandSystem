using Dignus.Actor.Core.Messages;

namespace Dignus.Commands.Messages
{
    internal struct RunCommandMessage(string commandLine) : IActorMessage
    {
        public string CommandLine { get; } = commandLine;
    }
}
