using CommandSystem.Attributes;
using CommandSystem.Middlewares;
using CommandSystem.Net.Components;
using CommandSystem.Net.Handler;
using CommandSystem.Net.Middlewares;
using CommandSystem.Net.PacketHandler;
using CommandSystem.Net.Protocol;
using CommandSystem.Net.Serializer;
using Dignus.Framework.Pipeline;
using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;
using System.Reflection;

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
            ProtocolPipelineInvoker<CSPipeContext, CSProtocolHandler, string>.Use<CSProtocol>((method, pipe) =>
            {
                var filters = method.GetCustomAttributes<ActionAttribute>();
                var orderedFilters = filters.OrderBy(r => r.Order).ToList();

                var middlewarePipeline = new RefMiddlewarePipeline<CSPipeContext>();

                var actionMiddleware = new ActionAttributeMiddleware(orderedFilters);

                pipe.Use(actionMiddleware.Invoke);

            });
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
                new ServerPacketHandler(handler),
                new List<ISessionComponent>()
                {
                    handler,
                    new AuthenticationComponent()
                });
        }
    }
}
