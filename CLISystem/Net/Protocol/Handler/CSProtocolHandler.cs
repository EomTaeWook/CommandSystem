using CLISystem.Models;
using CLISystem.Net.Protocol.Models;
using Kosher.Log;
using Kosher.Sockets;
using Kosher.Sockets.Interface;
using System.Text.Json;

namespace CLISystem.Net.Protocol.Handler
{
    public partial class CSProtocolHandler : ISessionComponent, IProtocolHandler
    {
        public Session Session { get; private set; }
        public void Dispose()
        {
        }
        
        public void SetSession(Session session)
        {
            Session = session;
        }
        public T DeserializeBody<T>(string body)
        {
            return JsonSerializer.Deserialize<T>(body);
        }
        public void Process(RemoteCommand remoteCommand)
        {
            if(string.IsNullOrEmpty(remoteCommand.Cmd) == true)
            {
                LogHelper.Error($"command is empty");
                {
                    var packet = Packet.MakePacket((ushort)SCProtocol.RemoteCommandResponse,
                    new RemoteCommandResponse()
                    {
                        Ok = false,
                        Body = "command is empty!"
                    });
                    Session.Send(packet);
                }
                
                return;
            }
            else
            {
                NetCLIModule.Instance.ProcessCommnad(remoteCommand.Cmd, out string body);
                var packet = Packet.MakePacket((ushort)SCProtocol.RemoteCommandResponse,
                    new RemoteCommandResponse()
                    {
                        Ok = true,
                        Body = body
                    });
                Session.Send(packet);
            }
            
        }
        public void Process(GetModuleInfo _)
        {
            var config = NetCLIModule.Instance._builder.GetService<Configuration>();
            var item = new GetModuleInfoResponse()
            {
                ModuleName = config.ModuleName
            };
            Session.Send(Packet.MakePacket((ushort)SCProtocol.GetModuleInfoResponse, item));
        }
    }
}
