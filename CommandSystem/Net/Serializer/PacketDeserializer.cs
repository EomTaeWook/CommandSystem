using Dignus.Collections;
using Dignus.Log;
using Dignus.Sockets.Extensions;
using Dignus.Sockets.Interface;
using System.Text;

namespace CommandSystem.Net.Serializer
{
    internal class PacketDeserializer<THandler> : IPacketDeserializer where THandler : IProtocolHandler<string>
    {
        private readonly THandler _handler;
        private const int ProtocolSize = sizeof(ushort);
        private const int LengthSize = sizeof(int);

        public PacketDeserializer(THandler handler)
        {
            _handler = handler;
        }

        public bool IsCompletePacketInBuffer(ArrayQueue<byte> buffer)
        {
            if (buffer.Count < LengthSize)
            {
                return false;
            }
            var packetSize = BitConverter.ToInt32(buffer.Peek(LengthSize));
            return buffer.Count >= packetSize + LengthSize;
        }

        public void Deserialize(ArrayQueue<byte> buffer)
        {
            var packetSize = BitConverter.ToInt32(buffer.Read(LengthSize));

            var bytes = buffer.Read(packetSize);

            int protocol = BitConverter.ToInt16(bytes);

            var body = Encoding.UTF8.GetString(bytes, ProtocolSize, bytes.Length - ProtocolSize);

            if (_handler.CheckProtocol(protocol) == true)
            {
                _handler.Process(protocol, body);
            }
            else
            {
                LogHelper.Error($"[{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff")}] not found protocol : {protocol}");
            }
        }
    }
}
