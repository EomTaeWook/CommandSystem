using CommandSystem.Net.Components;
using CommandSystem.Net.Handler;
using CommandSystem.Net.Protocol;
using CommandSystem.Net.Serializer;
using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;

namespace CommandSystem.Net
{
    internal class NetServerModule
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
        public NetServerModule(ServerCmdModule cmdModule)
        {
            HandlerFilterInvoker<CSProtocolHandler>.BindProtocol<CSProtocol>();
            _cmdModule = cmdModule;
        }
        public void Run(int port)
        {
            _server = new InnerServer(new SessionConfiguration(MakeSerializersFunc));
            _server.Start("", port, 100);
            LogHelper.Info($"*** command server start : port {port} ***");
        }
        public SessionSetup MakeSerializersFunc()
        {
            CSProtocolHandler handler = new(_cmdModule);

            return new SessionSetup(new PacketSerializer(),
                new PacketDeserializer<CSProtocolHandler>(handler),
                new List<ISessionComponent>()
                {
                    handler,
                    new AuthenticationComponent()
                });
        }
    }
}
