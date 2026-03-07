using Dignus.Actor.Core.Messages;

namespace CommandSystem.Network.Messages
{
    public class CommandLineMessage : IActorMessage
    {
        public ArraySegment<byte> CommandLine { get; set; }
    }
}
