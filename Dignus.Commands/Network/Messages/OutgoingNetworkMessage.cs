using Dignus.Actor.Network.Messages;

namespace Dignus.Commands.Network.Messages
{
    public struct OutgoingNetworkMessage : INetworkActorMessage
    {
        public byte[] Bytes { get; set; }
    }
}
