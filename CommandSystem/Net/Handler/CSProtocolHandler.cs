using CommandSystem.Models;
using CommandSystem.Net.Protocol;
using CommandSystem.Net.Protocol.Models;
using Dignus.DependencyInjection.Attribute;
using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Attribute;
using Dignus.Sockets.Interface;
using System.Text.Json;

namespace CommandSystem.Net.Handler
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
    public class CSProtocolHandler : ISessionHandler, IProtocolHandler<string>
    {
        private readonly ServerCmdModule _cmdServerModule;

        public CSProtocolHandler(ServerCmdModule cmdServerModule)
        {
            _cmdServerModule = cmdServerModule;
        }
        public Session Session { get; private set; }
        public void Dispose()
        {
            Session = null;
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
            _cmdServerModule.CacelCommand(cancelCommand.JobId);
        }

        [ProtocolName("RemoteCommand")]
        public void Process(RemoteCommand remoteCommand)
        {
            Packet packet;
            if (string.IsNullOrEmpty(remoteCommand.Cmd) == true)
            {
                LogHelper.Error($"command is empty");

                packet = Packet.MakePacket((ushort)SCProtocol.RemoteCommandResponse,
                    new RemoteCommandResponse()
                    {
                        Ok = false,
                        ErrorMessage = "command is empty!"
                    });
                Session.Send(packet);

                return;
            }

            var jobId = _cmdServerModule.JobManager.AddJob(remoteCommand.Cmd, Session);

            packet = Packet.MakePacket((ushort)SCProtocol.RemoteCommandResponse,
                new RemoteCommandResponse()
                {
                    Ok = true,
                    JobId = jobId
                });
            Session.Send(packet);


            //var body = await _cmdServerModule.ProcessCommandAsync(remoteCommand.Cmd);

        }

        [ProtocolName("GetModuleInfo")]
        public void Process(GetModuleInfo _)
        {
            var config = _cmdServerModule._builder._commandContainer.Resolve<Configuration>();
            var item = new GetModuleInfoResponse()
            {
                ModuleName = config.ModuleName
            };
            Session.Send(Packet.MakePacket((ushort)SCProtocol.GetModuleInfoResponse, item));
        }
    }
}
