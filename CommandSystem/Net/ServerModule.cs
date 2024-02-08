using CommandSystem.Models;
using CommandSystem.Net.Handler;
using CommandSystem.Net.Protocol;
using CommandSystem.Net.Serializer;
using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Interface;

namespace CommandSystem.Net
{
    internal class ServerModule
    {
        internal class InternalServer : ServerBase
        {
            public InternalServer(SessionCreator sessionCreator) : base(sessionCreator)
            {
            }
            protected override void OnAccepted(Session session)
            {
            }

            protected override void OnDisconnected(Session session)
            {
            }
        }

        private InternalServer _server;
        private readonly ServerCmdModule _cmdModule;
        public ServerModule(ServerCmdModule cmdModule)
        {
            ProtocolToHandlerMapper<CSProtocolHandler, string>.BindProtocol<CSProtocol>();

            _cmdModule = cmdModule;
        }
        public void Run(int port, Builder builder)
        {
            Configuration configuration = builder._commandContainer.Resolve<Configuration>();

            _server = new InternalServer(new SessionCreator(MakeSerializersFunc));
            _server.Start(port);
            LogHelper.Info($"*** command server start : port {port} ***");
        }
        public Tuple<IPacketSerializer, IPacketDeserializer, ICollection<ISessionHandler>> MakeSerializersFunc()
        {
            CSProtocolHandler handler = new(_cmdModule);

            return Tuple.Create<IPacketSerializer,
                IPacketDeserializer,
                ICollection<ISessionHandler>>(new PacketSerializer(),
                new ClientPacketDeserializer(handler),
                new List<ISessionHandler>() { handler });
        }
    }
}
