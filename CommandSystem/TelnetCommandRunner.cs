using CommandSystem.Interfaces;
using CommandSystem.Internals;
using CommandSystem.Network;
using CommandSystem.Network.Messages;
using Dignus.Actor.Core.Actors;
using Dignus.Actor.Core.Messages;
using System.Text;

namespace CommandSystem
{
    public class TelnetCommandRunner(string moduleName = null, int port = 23) : 
        CommandModuleBase(moduleName), 
        ITelnetServerEventHandler
    {
        private TelnetServer _telnetServer;
        private readonly int _port = port;
        public void Build()
        {
            BuildInternal();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _telnetServer = new TelnetServer(GetModuleName(), _serviceProvider, this);
        }
        public void Run()
        {
            if(_telnetServer == null)
            {
                throw new InvalidOperationException("the TelnetServer instance is null. Call the Build method first.");
            }
            _telnetServer.Start(_port);
        }

        public void Close()
        {
            _telnetServer.Close();
        }

        public void OnAccepted(IActorRef connectedActorRef)
        {
            // 텔넷 협상을 위한 Interpret As Command (IAC) 바이트 정의
            byte interpretAsCommand = 0xFF; // IAC
            byte willCommand = 0xFB;        // WILL
            byte echoOption = 0x01;         // ECHO
            byte suppressGoAheadOption = 0x03; // SUPPRESS GO AHEAD

            byte[] telnetNegotiation =
            [
                interpretAsCommand, willCommand, echoOption,
                interpretAsCommand, willCommand, suppressGoAheadOption
            ];

            connectedActorRef.Post(new RawNetworkMessage()
            {
                Bytes = telnetNegotiation
            });

            var bytes = Encoding.GetEncoding(949).GetBytes($"{moduleName} > ");
            var promptMessage = new RawNetworkMessage()
            {
                Bytes = bytes
            };
            connectedActorRef.Post(promptMessage);
        }

        public void OnDisconnected(IActorRef connectedActorRef)
        {
            
        }

        public void OnDeadLetterMessage(DeadLetterMessage deadLetterMessage)
        {
            
        }
    }
}