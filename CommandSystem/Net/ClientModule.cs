﻿using CommandSystem.Net.Handler;
using CommandSystem.Net.Protocol;
using CommandSystem.Net.Protocol.Models;
using CommandSystem.Net.Serializer;
using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Interfaces;

namespace CommandSystem.Net
{
    internal class ClientModule
    {
        internal class InternalClient : ClientBase
        {
            public bool IsConnect { get; private set; }
            Action _onDisconnect;
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

        readonly InternalClient _client;
        readonly ClientCmdModule _clientModule;
        private string _ip;
        private int _port;
        public ClientModule(ClientCmdModule clientModule)
        {
            _clientModule = clientModule;
            HandlerFilterInvoker<SCProtocolHandler>.BindProtocol<SCProtocol>();

            _client = new InternalClient(new SessionConfiguration(MakeSerializersFunc),
                () =>
                {
                    var _ = ReconnectAsync();
                });
        }

        private async Task ReconnectAsync()
        {
            await Task.Delay(5000);
            LogHelper.Info("reconnect command server...");
            _client.Connect(_ip, _port);
            if (_client.IsConnect == false)
            {
                _ = ReconnectAsync();
            }
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
        public Tuple<IPacketSerializer, IPacketHandler, ICollection<ISessionComponent>> MakeSerializersFunc()
        {
            var handler = new SCProtocolHandler(_clientModule);

            return Tuple.Create<IPacketSerializer,
                IPacketHandler,
                ICollection<ISessionComponent>>(new PacketSerializer(),
                new PacketDeserializer<SCProtocolHandler>(handler),
                new List<ISessionComponent>()
                {
                    handler
                });
        }
    }
}
