using CLISystem.Models;
using CLISystem.Net.Protocol.Handler;
using CLISystem.Net.Serializer;
using Kosher.Log;
using Kosher.Sockets;
using Kosher.Sockets.Interface;
using System.Collections.Generic;

namespace CLISystem.Net
{
    internal class ServerModule
    {
        internal class InternalServer : BaseServer
        {
            private readonly string _moduleName;
            public InternalServer(string moduleName, SessionCreator sessionCreator) : base(sessionCreator)
            {
                _moduleName = moduleName;
            }
            protected override void OnAccepted(Session session)
            {
                LogHelper.Debug($"[{_moduleName}]connect session");
            }

            protected override void OnDisconnected(Session session)
            {
                LogHelper.Debug($"[{_moduleName}]disconnect session");
            }
        }

        InternalServer _server;
        public ServerModule()
        {
        }
        public void Run(int port, Builder builder)
        {
            CSProtocolHandler.Init();
            Configuration configuration = builder.GetService<Configuration>();
            _server = new InternalServer(configuration.ModuleName, new SessionCreator(MakeSerializersFunc));
            _server.Start(port);
            Console.WriteLine($"*** cli server start : port {port} ***");
        }

        public Tuple<IPacketSerializer, IPacketDeserializer, ICollection<ISessionComponent>> MakeSerializersFunc()
        {
            CSProtocolHandler handler = new CSProtocolHandler();
            var handlers = new List<IProtocolHandler>() { handler };

            return Tuple.Create<IPacketSerializer,
                IPacketDeserializer,
                ICollection<ISessionComponent>>(new PacketSerializer(),
                new PacketDeserializer(handlers),
                new List<ISessionComponent>() { handler });
        }
    }
}
