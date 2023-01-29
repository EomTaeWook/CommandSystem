using Kosher.Sockets.Interface;

namespace CLISystem.Net.Serializer
{
    internal class PacketSerializer : IPacketSerializer
    {
        public ArraySegment<byte> MakeSendBuffer(IPacket packet)
        {
            var sendPacket = packet as Packet;

            var iSize = sizeof(int);
            var packetSize = sendPacket.GetLength() + iSize;

            var buffer = new byte[packetSize];
            Buffer.BlockCopy(BitConverter.GetBytes(sendPacket.GetLength()), 0, buffer, 0, iSize);
            Buffer.BlockCopy(sendPacket.Body, 0, buffer, iSize, sendPacket.Body.Length);
            return new ArraySegment<byte>(buffer);
        }
    }
}
