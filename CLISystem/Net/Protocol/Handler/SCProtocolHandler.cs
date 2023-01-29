using CLISystem.Models;
using CLISystem.Net.Protocol.Models;
using Kosher.Sockets;
using Kosher.Sockets.Interface;
using System.Text.Json;

namespace CLISystem.Net.Protocol.Handler
{
    public partial class SCProtocolHandler : ISessionComponent, IProtocolHandler
    {
        public Session Session { get; private set; }
        
        public void Dispose()
        {
            
        }
        public void Process(RemoteCommandResponse res)
        {
            Console.WriteLine(res.Body);
        }
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
