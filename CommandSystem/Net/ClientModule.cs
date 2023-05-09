using CommandSystem.Net.Handler;
using CommandSystem.Net.Protocol;
using CommandSystem.Net.Protocol.Models;
using CommandSystem.Net.Serializer;
using Kosher.Sockets;
using Kosher.Sockets.Interface;
using Kosher.Sockets.ObjectPool;

namespace CommandSystem.Net
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
        NetClientModule _netClientCLIModule;
        public void SetNetCLIModule(NetClientModule netClientCLIModule)
        {
            _netClientCLIModule = netClientCLIModule;
        }
        public void Run(string ip, int port)
        {
            HandlerBinder<SCProtocolHandler, string>.Bind<SCProtocol>();
            _client = new InternalClient(new SessionCreator(MakeSerializersFunc, LohMemoryPool<byte>.Instance));
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
            var handler = new SCProtocolHandler(_netClientCLIModule);

            return Tuple.Create<IPacketSerializer,
                IPacketDeserializer,
                ICollection<ISessionComponent>>(new PacketSerializer(),
                new PacketDeserializer(handler),
                new List<ISessionComponent>()
                {
                    handler
                });
        }
    }
}
