using CommandSystem.Models;
using CommandSystem.Net.Protocol;
using CommandSystem.Net.Protocol.Models;
using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Attribute;
using Dignus.Sockets.Interface;
using System.Text.Json;

namespace CommandSystem.Net.Handler
{
    public class CSProtocolHandler : ISessionComponent, IProtocolHandler<string>
    {
        private readonly ServerCmdModule _cmdServerModule;
        public CSProtocolHandler(ServerCmdModule cmdServerModule)
        {
            _cmdServerModule = cmdServerModule;
        }
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

        [ProtocolName("CancelCommand")]
        public void Process(CancelCommand cancelCommand)
        {
            _cmdServerModule.CacelCommand();
        }


        [ProtocolName("RemoteCommand")]
        public async Task ProcessAsync(RemoteCommand remoteCommand)
        {
            if (string.IsNullOrEmpty(remoteCommand.Cmd) == true)
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
                var body = await _cmdServerModule.ProcessCommandAsync(remoteCommand.Cmd);
                Packet packet = Packet.MakePacket((ushort)SCProtocol.RemoteCommandResponse,
                    new RemoteCommandResponse()
                    {
                        Ok = true,
                        Body = body
                    });

                Session.Send(packet);

            }
        }
        
        [ProtocolName("GetModuleInfo")]
        public void Process(GetModuleInfo _)
        {
            var config = _cmdServerModule._builder.GetService<Configuration>();
            var item = new GetModuleInfoResponse()
            {
                ModuleName = config.ModuleName
            };
            Session.Send(Packet.MakePacket((ushort)SCProtocol.GetModuleInfoResponse, item));
        }
    }
}
