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
    internal class ClientPacketHandler : StatelessPacketHandlerBase
    {
        private readonly SCProtocolHandler _handler;
        private const int SizeToInt = sizeof(int);
        private const int ProtocolSize = sizeof(ushort);
        public ClientPacketHandler(SCProtocolHandler handler)
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

        public async override Task ProcessPacketAsync(ISession session, ArraySegment<byte> packet)
        {
            int protocol = BitConverter.ToInt16(packet);

            if (ProtocolHandlerMapper.ValidateProtocol<SCProtocolHandler>(protocol) == false)
            {
                LogHelper.Error($"[{DateTime.Now:yyyy-MM-dd hh:mm:ss:fff}] not found protocol : {protocol}");
                return;
            }
            var body = Encoding.UTF8.GetString(packet.Array, packet.Offset + ProtocolSize, packet.Count - ProtocolSize);

            SCPipeContext context = new()
            {
                Body = body,
                Handler = _handler,
                Protocol = protocol,
            };

            await ProtocolPipelineInvoker<SCPipeContext, SCProtocolHandler, string>.ExecuteAsync(protocol, ref context);
        }
    }
}
