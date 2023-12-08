using CommandSystem.Net.Handler;
using Dignus.Collections;
using Dignus.Log;
using Dignus.Sockets.Extensions;
using Dignus.Sockets.Interface;
using System.Text;

namespace CommandSystem.Net.Serializer
{
    internal abstract class PacketDeserializer : IPacketDeserializer
    {
        protected const int SizeToInt = sizeof(int);
        protected const int ProtocolSize = sizeof(ushort);
        public abstract void Deserialize(ArrayQueue<byte> buffer);
        public bool IsCompletePacketInBuffer(ArrayQueue<byte> buffer)
        {
            if (buffer.Count < SizeToInt)
            {
                return false;
            }
            var packetSize = BitConverter.ToInt32(buffer.Peek(SizeToInt));
            return buffer.Count >= packetSize + SizeToInt;
        }
    }

    internal class ClientPacketDeserializer : PacketDeserializer
    {
        private readonly CSProtocolHandler _handler;
        private const int LengthSize = sizeof(int);

        public ClientPacketDeserializer(CSProtocolHandler handler)
        {
            _handler = handler;
        }
        public override void Deserialize(ArrayQueue<byte> buffer)
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

    internal class ServerPacketDeserializer : PacketDeserializer
    {
        private readonly SCProtocolHandler _handler;
        private const int LengthSize = sizeof(int);

        public ServerPacketDeserializer(SCProtocolHandler handler)
        {
            _handler = handler;
        }
        public override void Deserialize(ArrayQueue<byte> buffer)
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
