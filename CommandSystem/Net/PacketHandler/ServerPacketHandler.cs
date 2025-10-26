using CommandSystem.Net.Handler;
using CommandSystem.Net.Middlewares;
using Dignus.Collections;
using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;
using Dignus.Sockets.Processing;
using System.Text;

namespace CommandSystem.Net.PacketHandler
{
    public class ServerPacketHandler : StatelessPacketHandlerBase
    {
        private readonly CSProtocolHandler _handler;
        private const int SizeToInt = sizeof(int);
        private const int ProtocolSize = sizeof(ushort);
        public ServerPacketHandler(CSProtocolHandler handler)
        {
            _handler = handler;
        }

        public override bool TakeReceivedPacket(ISession session, ArrayQueue<byte> buffer, out ArraySegment<byte> packet, out int consumedBytes)
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

        public override void ProcessPacket(ISession session, in ArraySegment<byte> packet)
        {
            int protocol = BitConverter.ToInt16(packet);

            if (ProtocolHandlerMapper.ValidateProtocol<CSProtocolHandler>(protocol) == false)
            {
                LogHelper.Error($"[{DateTime.Now:yyyy-MM-dd hh:mm:ss:fff}] not found protocol : {protocol}");
                return;
            }
            var body = Encoding.UTF8.GetString(packet.Array, packet.Offset + ProtocolSize, packet.Count - ProtocolSize);

            CSPipeContext context = new()
            {
                Body = body,
                Handler = _handler,
                Protocol = protocol,
                Session = session
            };

            ProtocolPipelineInvoker<CSPipeContext, CSProtocolHandler, string>.Execute(protocol, ref context);
        }
    }
}
