using Dignus.Actor.Core.Messages;

namespace Dignus.Commands.Messages
{
    internal readonly struct RunCommandMessage(string currentPath, string commandLine) : IActorMessage
    {
        public string CommandLine { get; } = commandLine;

        public string CurrentPath { get; } = currentPath;
    }
}
