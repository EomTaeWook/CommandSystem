using Dignus.Actor.Network.Messages;

namespace CommandSystem.Network.Messages
{
    public struct RawNetworkMessage : INetworkActorMessage
    {
        public byte[] Bytes { get; set; }
    }
}
