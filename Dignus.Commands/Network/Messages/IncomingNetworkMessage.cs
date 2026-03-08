using Dignus.Actor.Core.Messages;

namespace Dignus.Commands.Network.Messages
{
    internal struct IncomingNetworkMessage : IActorMessage
    {
        public byte[] Bytes { get; set; }
    }
}
