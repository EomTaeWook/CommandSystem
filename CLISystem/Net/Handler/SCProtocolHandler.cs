using CLISystem.Models;
using CLISystem.Net.Protocol.Models;
using Kosher.Sockets;
using Kosher.Sockets.Attribute;
using Kosher.Sockets.Interface;
using System.Text.Json;

namespace CLISystem.Net.Handler
{
    public partial class SCProtocolHandler : ISessionComponent, IProtocolHandler<string>
    {
        public Session Session { get; private set; }

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
            var config = NetClientCLIModule.Instance._builder.GetService<Configuration>();

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
