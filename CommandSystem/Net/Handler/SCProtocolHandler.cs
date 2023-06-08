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

        NetClientModule _cliModule;
        public SCProtocolHandler(NetClientModule cliModule)
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
        }
        [ProtocolName("GetModuleInfoResponse")]
        public void Process(GetModuleInfoResponse res)
        {
            var config = _cliModule._builder.GetService<Configuration>();

            config.ModuleName = res.ModuleName;
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
