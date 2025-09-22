using CommandSystem.Extensions;
using CommandSystem.Net.Protocol.Models;
using Dignus.DependencyInjection.Attributes;
using Dignus.Sockets.Attributes;
using Dignus.Sockets.Interfaces;
using System.Text.Json;

namespace CommandSystem.Net.Handler
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
    public partial class SCProtocolHandler : ISessionComponent, IProtocolHandler<string>
    {
        private SessionContext _sessionContext;
        public ISession Session { get; private set; }

        readonly ClientCmdModule _cliModule;

        public SCProtocolHandler(ClientCmdModule cliModule)
        {
            _cliModule = cliModule;
        }

        public void Dispose()
        {
            Session = null;
        }
        [ProtocolName("RemoteCommandResponse")]
        public void Process(RemoteCommandResponse packet)
        {
            if (packet.Ok == false)
            {
                Console.WriteLine(packet.ErrorMessage);
                _cliModule.ResetJobId();
                Task.Run(_cliModule.DisplayPrompt);
            }
            else
            {
                _cliModule.SetJobId(packet.JobId);
            }
        }
        [ProtocolName("GetModuleInfoResponse")]
        public void GetModuleInfoResponse(GetModuleInfoResponse packet)
        {
            _cliModule.SetModuleName($"{packet.ModuleName}@{_cliModule.IpString}");
            _cliModule.DisplayPrompt();
        }
        [ProtocolName("CancelCommandResponse")]
        public void Process(CancelCommandResponse packet)
        {
            _cliModule.ResetJobId();
            _cliModule.DisplayPrompt();
        }
        [ProtocolName("NotifyConsoleText")]
        public void Process(NotifyConsoleText packet)
        {
            Console.Write(packet.ConsoleText);
        }
        [ProtocolName("CompleteRemoteCommand")]
        public void Process(CompleteRemoteCommand packet)
        {
            _cliModule.ResetJobId();
            Console.WriteLine(packet.ConsoleText);
            Task.Run(_cliModule.DisplayPrompt);
        }
        public void SetSession(ISession session)
        {
            Session = session;
            _sessionContext = new SessionContext();
            _sessionContext.SetSession(session);
        }
        public T DeserializeBody<T>(string body)
        {
            return JsonSerializer.Deserialize<T>(body);
        }
    }
}
