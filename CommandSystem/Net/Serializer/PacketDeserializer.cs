using Dignus.Collections;
using Dignus.Log;
using Dignus.Sockets.Extensions;
using Dignus.Sockets.Interface;
using System.Text;

namespace CommandSystem.Net.Serializer
{
    internal class PacketDeserializer : IPacketDeserializer
    {
        private readonly IProtocolHandler<string> _handler;
        private const int LengthSize = sizeof(int);

        public PacketDeserializer(IProtocolHandler<string> handler)
        {
            _handler = handler;
        }

        public bool IsTakedCompletePacket(ArrayList<byte> buffer)
        {
            if(buffer.Count < LengthSize)
            {
                return false;
            }
            var packetSize = BitConverter.ToInt32(buffer.Peek(LengthSize));

            LogHelper.Debug($"[{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss:fff")}] buffer size : {buffer.Count}");

            return buffer.Count >= packetSize + LengthSize;

        }

        public void Deserialize(ArrayList<byte> buffer)
        {
            var packetSize = BitConverter.ToInt32(buffer.Read(LengthSize));

            var bytes = buffer.Read(packetSize);

            int protocol = BitConverter.ToInt16(bytes);

            var body = Encoding.UTF8.GetString(bytes, LengthSize, bytes.Length - LengthSize);

            if (_handler.CheckProtocol(protocol) == true)
            {
                _handler.Process(protocol, body);
            }
        }
    }
}
