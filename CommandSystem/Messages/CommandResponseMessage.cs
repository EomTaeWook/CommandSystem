using Dignus.Actor.Core.Messages;

namespace CommandSystem.Messages
{
    public struct CommandResponseMessage : IActorMessage
    {
        public string Content { get; set; }
    }
}
