using CommandSystem.Net.Handler;
using CommandSystem.Net.Protocol;
using CommandSystem.Net.Protocol.Models;
using CommandSystem.Net.Serializer;
using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;

namespace CommandSystem.Net
{
    internal class NetClientModule
    {
        internal class InternalClient : ClientBase
        {
            public bool IsConnect { get; private set; }
            private readonly Action _onDisconnect;
            public InternalClient(SessionConfiguration sessionConfiguration, Action onDisconnect) : base(sessionConfiguration)
            {
                _onDisconnect = onDisconnect;
            }

            protected override void OnConnected(ISession session)
            {
                IsConnect = true;
                LogHelper.Info("connect command server");
                var packet = Packet.MakePacket((ushort)CSProtocol.GetModuleInfo, new GetModuleInfo());
                Send(packet);
            }

            protected override void OnDisconnected(ISession session)
            {
                LogHelper.Info("disconnect command server");
                IsConnect = false;
                _onDisconnect?.Invoke();
            }
        }

        public event Action<NetClientModule> Disconnected;
        private readonly InternalClient _client;
        private readonly ClientCmdModule _clientModule;
        private string _ip;
        private int _port;
        public NetClientModule(ClientCmdModule clientModule)
        {
            _clientModule = clientModule;
            HandlerFilterInvoker<SCProtocolHandler>.BindProtocol<SCProtocol>();

            _client = new InternalClient(new SessionConfiguration(MakeSerializersFunc),
                () =>
                {
                    Disconnected?.Invoke(this);
                });
        }
        public void Run(string ip, int port)
        {
            _ip = ip;
            _port = port;
            _client.Connect(_ip, _port);
        }
        public void SendCommand(string line)
        {
            _client.Send(Packet.MakePacket((ushort)CSProtocol.RemoteCommand, new RemoteCommand()
            {
                Cmd = line
            }));
        }
        public void CacelCommand(int jobId)
        {
            _client.Send(Packet.MakePacket((ushort)CSProtocol.CancelCommand, new CancelCommand() { JobId = jobId }));
        }
        public SessionSetup MakeSerializersFunc()
        {
            var handler = new SCProtocolHandler(_clientModule);

            return new SessionSetup(new PacketSerializer(),
                new PacketDeserializer<SCProtocolHandler>(handler),
                new List<ISessionComponent>()
                {
                    handler
                });
        }
    }
}
