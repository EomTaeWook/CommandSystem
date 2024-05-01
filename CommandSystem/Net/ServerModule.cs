using CommandSystem.Net.Handler;
using CommandSystem.Net.Protocol;
using CommandSystem.Net.Serializer;
using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;

namespace CommandSystem.Net
{
    class Test : ISessionComponent
    {
        public void Dispose()
        {

        }

        public void SetSession(ISession session)
        {

        }
    }

    internal class ServerModule
    {
        internal class InnerServer(SessionConfiguration sessionConfiguration) : ServerBase(sessionConfiguration)
        {
            protected override void OnAccepted(ISession session)
            {
                session.AddSessionComponent(new Test());
            }

            protected override void OnDisconnected(ISession session)
            {
            }
        }

        private InnerServer _server;
        private readonly ServerCmdModule _cmdModule;
        public ServerModule(ServerCmdModule cmdModule)
        {
            HandlerFilterInvoker<CSProtocolHandler>.BindProtocol<CSProtocol>();
            _cmdModule = cmdModule;
        }
        public void Run(int port)
        {
            _server = new InnerServer(new SessionConfiguration(MakeSerializersFunc));
            _server.Start("", port, ProtocolType.Tcp, 100);
            LogHelper.Info($"*** command server start : port {port} ***");
        }
        public Tuple<IPacketSerializer, IPacketDeserializer, ICollection<ISessionComponent>> MakeSerializersFunc()
        {
            CSProtocolHandler handler = new(_cmdModule);

            return Tuple.Create<IPacketSerializer,
                IPacketDeserializer,
                ICollection<ISessionComponent>>(new PacketSerializer(),
                new ClientPacketDeserializer(handler),
                new List<ISessionComponent>() { handler });
        }
    }
}
