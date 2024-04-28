using CommandSystem.Attributes;
using CommandSystem.Net.Protocol;
using CommandSystem.Net.Protocol.Models;
using Dignus.DependencyInjection.Attributes;
using Dignus.Log;
using Dignus.Sockets.Attributes;
using Dignus.Sockets.Interfaces;
using System.Text.Json;

namespace CommandSystem.Net.Handler
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
    public class CSProtocolHandler : IProtocolHandler<string>, IProtocolHandlerContext, ISessionHandler
    {
        private readonly ServerCmdModule _cmdServerModule;
        private SessionContext _sessionContext;
        private ISession _session;
        public CSProtocolHandler(ServerCmdModule cmdServerModule)
        {
            _cmdServerModule = cmdServerModule;
        }
        public void Dispose()
        {
            _sessionContext = null;
        }

        public T DeserializeBody<T>(string body)
        {
            return JsonSerializer.Deserialize<T>(body);
        }
        [Authorization]
        [ProtocolName("CancelCommand")]
        public void Process(CancelCommand cancelCommand)
        {
            _cmdServerModule.CancelCommand(cancelCommand.JobId);
        }
        [Authorization]
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
                _sessionContext.Send(packet);

                return;
            }

            var jobId = _cmdServerModule.JobManager.AddJob(remoteCommand.Cmd, _sessionContext);

            packet = Packet.MakePacket((ushort)SCProtocol.RemoteCommandResponse,
                new RemoteCommandResponse()
                {
                    Ok = true,
                    JobId = jobId
                });
            _sessionContext.Send(packet);
        }

        [ProtocolName("GetModuleInfo")]
        public void Process(GetModuleInfo _)
        {
            var item = new GetModuleInfoResponse()
            {
                ModuleName = _cmdServerModule.GetModuleName()
            };
            _sessionContext = new SessionContext();
            _sessionContext.SetSession(_session);
            _sessionContext.Send(Packet.MakePacket((ushort)SCProtocol.GetModuleInfoResponse, item));
        }

        public SessionContext GetSessionContext()
        {
            return _sessionContext;
        }

        public void SetSession(ISession session)
        {
            _session = session;
        }
    }
}
