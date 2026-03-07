using CommandSystem.Actors;
using CommandSystem.Interfaces;
using CommandSystem.Internals;
using CommandSystem.Network.Decoder;
using CommandSystem.Network.Serializer;
using Dignus.Actor.Core.Actors;
using Dignus.Actor.Core.Messages;
using Dignus.Actor.Network;
using Dignus.Actor.Network.Options;
using System.Net;

namespace CommandSystem.Network
{
    internal class TelnetServer
    {
        internal class InnerTelnetServer(string moduleName,
            IServiceProvider serviceProvider,
            ITelnetServerEventHandler telnetServerEventHandler,
            ServerOptions options) : 
            TcpServerBase<TelnetClientActor>(CommandActorSystem.Instance, options)
        {
            protected override TelnetClientActor CreateSessionActor()
            {
                var executionActorRef = CommandActorSystem.Instance.Spawn(() =>
                {
                    return new CommandExecutionActor(serviceProvider, null);
                });

                return new TelnetClientActor(executionActorRef, moduleName);
            }

            protected override void OnAccepted(IActorRef connectedActorRef)
            {
                telnetServerEventHandler.OnAccepted(connectedActorRef);
            }

            protected override void OnDeadLetterMessage(DeadLetterMessage deadLetterMessage)
            {
                telnetServerEventHandler.OnDeadLetterMessage(deadLetterMessage);
            }

            protected override void OnDisconnected(IActorRef connectedActorRef)
            {
                telnetServerEventHandler.OnDisconnected(connectedActorRef);
            }
        }

        private readonly InnerTelnetServer _server;

        public TelnetServer(string moduleName,
            IServiceProvider serviceProvider,
            ITelnetServerEventHandler  telnetServerEventHandler)
        {
            _server = new InnerTelnetServer(
                moduleName,
                serviceProvider,
                telnetServerEventHandler,
                new ServerOptions()
            {
                Network = new ActorNetworkOptions() 
                {
                    Decoder = new PacketDecoder(),
                    MessageSerializer = new PacketSerializer(),
                }
            });
        }
        public void Start(IPEndPoint ipEndPoint, int backlog = 200)
        {
            _server.Start(ipEndPoint, backlog);

        }
        public void Start(int port, int backlog = 200)
        {
            _server.Start(port, backlog);
        }
        public void Start(string ipString, int port, int backlog = 200)
        {
            IPEndPoint ipEndPoint = new(IPAddress.Parse(ipString), port);
            Start(ipEndPoint, backlog);
        }
        public void Close()
        {
            _server.Close();
        }
        public void Dispose()
        {
            _server.Dispose();
        }
    }
}
