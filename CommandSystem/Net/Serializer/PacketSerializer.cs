using Kosher.Collections;
using Kosher.Sockets.Interface;

namespace CommandSystem.Net.Serializer
{
    internal class PacketSerializer : IPacketSerializer
    {
        public ArraySegment<byte> MakeSendBuffer(IPacket packet)
        {
            var sendPacket = packet as Packet;

            var packetSize = sendPacket.GetLength();

            var buffer = new ArrayList<byte>();

            buffer.AddRange(BitConverter.GetBytes(packetSize));
            buffer.AddRange(BitConverter.GetBytes(sendPacket.Protocol));

            buffer.AddRange(sendPacket.Body);
            return buffer.ToArray();
        }
    }
}
