using CommandSystem.Net.Handler;
using CommandSystem.Net.Protocol;
using CommandSystem.Net.Serializer;
using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;

namespace CommandSystem.Net
{
    internal class ServerModule
    {
        internal class InnerServer(SessionConfiguration sessionConfiguration) : ServerBase(sessionConfiguration)
        {
            protected override void OnAccepted(ISession session)
            {
            }

            protected override void OnDisconnected(ISession session)
            {
            }
        }

        private InnerServer _server;
        private readonly ServerCmdModule _cmdModule;
        public ServerModule(ServerCmdModule cmdModule)
        {
            ProtocolHandlerMapper<CSProtocolHandler, string>.BindProtocol<CSProtocol>();
            _cmdModule = cmdModule;
        }
        public void Run(int port)
        {
            _server = new InnerServer(new SessionConfiguration(MakeSerializersFunc));
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
