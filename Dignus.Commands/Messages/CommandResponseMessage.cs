using Dignus.Actor.Core.Messages;

namespace Dignus.Commands.Messages
{
    public struct CommandResponseMessage : IActorMessage
    {
        public string Content { get; set; }
    }
}
