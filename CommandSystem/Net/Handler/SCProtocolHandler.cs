using CommandSystem.Models;
using CommandSystem.Net.Protocol.Models;
using Dignus.Sockets;
using Dignus.Sockets.Attribute;
using Dignus.Sockets.Interface;
using System.Text.Json;

namespace CommandSystem.Net.Handler
{
    public partial class SCProtocolHandler : ISessionComponent, IProtocolHandler<string>
    {
        public Session Session { get; private set; }

        readonly ClientCmdModule _cliModule;
        public SCProtocolHandler(ClientCmdModule cliModule)
        {
            _cliModule = cliModule;
        }

        public void Dispose()
        {

        }
        [ProtocolName("RemoteCommandResponse")]
        public void Process(RemoteCommandResponse res)
        {
            Console.WriteLine(res.Body);
            _cliModule.IsRequested = false;
            Task.Run(_cliModule.Prompt);
        }
        [ProtocolName("GetModuleInfoResponse")]
        public void GetModuleInfoResponse(GetModuleInfoResponse res)
        {
            var config = _cliModule._builder.GetService<Configuration>();

            config.ModuleName = res.ModuleName;
        }
        [ProtocolName("CancelCommandResponse")]
        public void Process(CancelCommandResponse res)
        {
            _cliModule.IsRequested = false;
            _cliModule.Prompt();
        }
        public void SetSession(Session session)
        {
            Session = session;
        }
        public T DeserializeBody<T>(string body)
        {
            return JsonSerializer.Deserialize<T>(body);
        }
    }
}
