using CommandSystem.Net.Handler;
using Dignus.Collections;
using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;
using Dignus.Sockets.Processing;
using System.Text;

namespace CommandSystem.Net.Serializer
{
    internal class PacketDeserializer<T> : SessionPacketProcessorBase where T : class, IProtocolHandlerBase, IProtocolHandler<string>, IProtocolHandlerContext
    {
        private readonly T _handler;
        protected const int SizeToInt = sizeof(int);
        protected const int ProtocolSize = sizeof(ushort);

        public PacketDeserializer(T handler)
        {
            _handler = handler;
        }
        public override bool TakeReceivedPacket(ArrayQueue<byte> buffer, out ArraySegment<byte> packet, out int consumedBytes)
        {
            packet = null;
            consumedBytes = 0;
            if (buffer.Count < SizeToInt)
            {
                return false;
            }

            var bodySize = BitConverter.ToInt32(buffer.Peek(SizeToInt));

            if (buffer.Count < bodySize + SizeToInt)
            {
                return false;
            }

            buffer.TryReadBytes(out _, SizeToInt);

            consumedBytes = bodySize;

            return buffer.TrySlice(out packet, bodySize);
        }

        public override void ProcessPacket(in ArraySegment<byte> packet)
        {
            int protocol = BitConverter.ToInt16(packet);

            if (ProtocolHandlerMapper.ValidateProtocol<T>(protocol) == false)
            {
                LogHelper.Error($"[{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff")}] not found protocol : {protocol}");
                return;
            }
            var body = Encoding.UTF8.GetString(packet.Array, packet.Offset + ProtocolSize, packet.Count - ProtocolSize);

            HandlerFilterInvoker<T>.ExecuteProtocolHandler(_handler,
                protocol,
                body).GetAwaiter().GetResult();
        }
    }
}
