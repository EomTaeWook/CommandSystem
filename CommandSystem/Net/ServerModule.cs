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
        NetServerModule _cliModule;
        public void Run(int port, Builder builder)
        {
            Configuration configuration = builder.GetService<Configuration>();
            _server = new InternalServer(configuration.ModuleName,
                new SessionCreator(MakeSerializersFunc));
            _server.Start(port);
            Console.WriteLine($"*** cli server start : port {port} ***");
        }
        public void SetNetCLIModule(NetServerModule netCLIModule)
        {
            _cliModule = netCLIModule;
            HandlerBinder<CSProtocolHandler, string>.Bind<CSProtocol>();

        }
        public Tuple<IPacketSerializer, IPacketDeserializer, ICollection<ISessionComponent>> MakeSerializersFunc()
        {
            CSProtocolHandler handler = new CSProtocolHandler(_cliModule);

            return Tuple.Create<IPacketSerializer,
                IPacketDeserializer,
                ICollection<ISessionComponent>>(new PacketSerializer(),
                new PacketDeserializer(handler),
                new List<ISessionComponent>() { handler });
        }
    }
}
