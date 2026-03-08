using Dignus.Actor.Core.Messages;

namespace Dignus.Commands.Messages
{
    internal struct ChangeDirectoryRequestMessage : IActorMessage
    {
        public string Path { get; set; }
    }
}
