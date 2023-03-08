using CLISystem.Models;
using CLISystem.Net.Handler;
using CLISystem.Net.Protocol;
using CLISystem.Net.Serializer;
using Kosher.Log;
using Kosher.Sockets;
using Kosher.Sockets.Interface;

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
        NetCLIModule _cliModule;
        public void Run(int port, Builder builder)
        {
            HandlerBinder<CSProtocolHandler, string>.Bind<SCProtocol>();
            Configuration configuration = builder.GetService<Configuration>();
            _server = new InternalServer(configuration.ModuleName, new SessionCreator(MakeSerializersFunc));
            _server.Start(port);
            Console.WriteLine($"*** cli server start : port {port} ***");
        }
        public void SetNetCLIModule(NetCLIModule netCLIModule)
        {
            _cliModule = netCLIModule;
        }
        public Tuple<IPacketSerializer, IPacketDeserializer, ICollection<ISessionComponent>> MakeSerializersFunc()
        {
            CSProtocolHandler handler = new CSProtocolHandler(_cliModule);
            var handlers = new List<IProtocolHandler<string>>() { handler };

            return Tuple.Create<IPacketSerializer,
                IPacketDeserializer,
                ICollection<ISessionComponent>>(new PacketSerializer(),
                new PacketDeserializer(handlers),
                new List<ISessionComponent>() { handler });
        }
    }
}
