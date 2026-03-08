using CommandSystem.Network.Messages;
using Dignus.Actor.Network.Messages;
using Dignus.Actor.Network.Serialization;
using Dignus.Collections;
using Dignus.Sockets.Interfaces;

namespace CommandSystem.Network.Serializer
{
    internal class MessageSerializer : IActorMessageSerializer
    {
        public ArraySegment<byte> MakeSendBuffer(IPacket packet)
        {
            var sendPacket = packet as Packet;
            var buffer = new ArrayQueue<byte>();
            buffer.AddRange(sendPacket.Body);
            return buffer.ToArray();
        }

        public ArraySegment<byte> MakeSendBuffer(INetworkActorMessage message)
        {
            var networkMessage = (RawNetworkMessage)message;
            return networkMessage.Bytes;
        }
    }
}
