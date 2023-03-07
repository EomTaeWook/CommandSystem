using CLISystem.Net.Handler;
using CLISystem.Net.Protocol;
using CLISystem.Net.Protocol.Models;
using CLISystem.Net.Serializer;
using Kosher.Sockets;
using Kosher.Sockets.Interface;

namespace CLISystem.Net
{
    internal class ClientModule
    {
        internal class InternalClient : BaseClient
        {
            public InternalClient(SessionCreator sessionCreator) : base(sessionCreator)
            {
            }

            protected override void OnConnected(Session session)
            {
                
            }

            protected override void OnDisconnected(Session session)
            {
                
            }
        }

        InternalClient _client;

        public void Run(string ip, int port)
        {
            HandlerBinder<SCProtocolHandler, string>.Bind<SCProtocol>();
            _client = new InternalClient(new SessionCreator(MakeSerializersFunc));
            _client.Connect(ip, port);
        }
        public void SendCommand(string line)
        {
            _client.Send(Packet.MakePacket((ushort)CSProtocol.RemoteCommand, new RemoteCommand()
            {
                Cmd = line
            }));
        }
        public Tuple<IPacketSerializer, IPacketDeserializer, ICollection<ISessionComponent>> MakeSerializersFunc()
        {
            var handler = new SCProtocolHandler();
            var handlers = new List<IProtocolHandler<string>>() { handler };

            return Tuple.Create<IPacketSerializer,
                IPacketDeserializer,
                ICollection<ISessionComponent>>(new PacketSerializer(),
                new PacketDeserializer(handlers),
                new List<ISessionComponent>() 
                {
                    handler
                });
        }
    }
}
