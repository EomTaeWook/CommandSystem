using CommandSystem.Network.Messages;
using Dignus.Actor.Core.Messages;
using Dignus.Actor.Network.Protocol;
using Dignus.Collections;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;

namespace CommandSystem.Network.Decoder
{
    public class PacketDecoder : IMessageDecoder
    {
        public IActorMessage Deserialize(ReadOnlySpan<byte> packet)
        {
            return new CommandLineMessage()
            {
                CommandLine = packet.ToArray()
            };
        }

        public bool TryFrame(ISession session, ArrayQueue<byte> buffer, out ArraySegment<byte> packet, out int consumedBytes)
        {
            consumedBytes = buffer.Count;
            return buffer.TrySlice(out packet, consumedBytes);
        }
    }
}
