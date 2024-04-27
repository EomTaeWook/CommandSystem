using CommandSystem.Extensions;
using CommandSystem.Net.Protocol.Models;
using Dignus.DependencyInjection.Attributes;
using Dignus.Sockets.Attributes;
using Dignus.Sockets.Interfaces;
using System.Text.Json;

namespace CommandSystem.Net.Handler
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
    public partial class SCProtocolHandler : ISessionHandler, IProtocolHandler<string>
    {
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
        public void Process(RemoteCommandResponse res)
        {
            if (res.Ok == false)
            {
                Console.WriteLine(res.ErrorMessage);
                _cliModule.ResetJobId();
                Task.Run(_cliModule.DisplayPrompt);
            }
            else
            {
                _cliModule.SetJobId(res.JobId);
            }
        }
        [ProtocolName("GetModuleInfoResponse")]
        public void GetModuleInfoResponse(GetModuleInfoResponse res)
        {
            _cliModule.SetModuleName($"{res.ModuleName}@{_cliModule.IpString}");
            _cliModule.DisplayPrompt();
        }
        [ProtocolName("CancelCommandResponse")]
        public void Process(CancelCommandResponse res)
        {
            _cliModule.ResetJobId();
            _cliModule.DisplayPrompt();
        }
        [ProtocolName("NotifyConsoleText")]
        public void Process(NotifyConsoleText notifyConsoleText)
        {
            Console.Write(notifyConsoleText.ConsoleText);
        }
        [ProtocolName("CompleteRemoteCommand")]
        public void Process(CompleteRemoteCommand completeRemoteCommand)
        {
            _cliModule.ResetJobId();
            Console.WriteLine(completeRemoteCommand.ConsoleText);
            Task.Run(_cliModule.DisplayPrompt);
        }
        public void SetSession(ISession session)
        {
            Session = session;
        }
        public T DeserializeBody<T>(string body)
        {
            return JsonSerializer.Deserialize<T>(body);
        }
    }
}
