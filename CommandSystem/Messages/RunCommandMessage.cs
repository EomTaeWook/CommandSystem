using Dignus.Actor.Core.Messages;

namespace CommandSystem.Messages
{
    internal struct RunCommandMessage(string commandLine) : IActorMessage
    {
        public string CommandLine { get; } = commandLine;
    }
}
