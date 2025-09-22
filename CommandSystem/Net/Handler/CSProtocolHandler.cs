using CommandSystem.Attributes;
using CommandSystem.Net.Components;
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
    public class CSProtocolHandler(ServerCmdModule cmdServerModule) : IProtocolHandler<string>, ISessionComponent
    {
        private SessionContext _sessionContext;
        private ISession _session;

        public void Dispose()
        {
        }

        public T DeserializeBody<T>(string body)
        {
            return JsonSerializer.Deserialize<T>(body);
        }
        [Authorization]
        [ProtocolName("CancelCommand")]
        public void Process(CancelCommand packet)
        {
            cmdServerModule.CancelCommand(packet.JobId);
        }
        [Authorization]
        [ProtocolName("RemoteCommand")]
        public void Process(RemoteCommand packet)
        {
            Packet sendPacket;
            if (string.IsNullOrEmpty(packet.Cmd) == true)
            {
                LogHelper.Error($"command is empty");

                sendPacket = Packet.MakePacket((ushort)SCProtocol.RemoteCommandResponse,
                    new RemoteCommandResponse()
                    {
                        Ok = false,
                        ErrorMessage = "command is empty!"
                    });
                _sessionContext.Send(sendPacket);

                return;
            }

            var jobId = cmdServerModule.JobManager.AddJob(packet.Cmd, _sessionContext);

            sendPacket = Packet.MakePacket((ushort)SCProtocol.RemoteCommandResponse,
                new RemoteCommandResponse()
                {
                    Ok = true,
                    JobId = jobId
                });
            _sessionContext.Send(sendPacket);
        }

        [ProtocolName("GetModuleInfo")]
        public void Process(GetModuleInfo _)
        {
            var item = new GetModuleInfoResponse()
            {
                ModuleName = cmdServerModule.GetModuleName()
            };
            foreach (var component in _session.GetSessionComponents())
            {
                if (component is AuthenticationComponent authComponent)
                {
                    authComponent.SetAuth();
                    break;
                }
            }
            _sessionContext.Send(Packet.MakePacket((ushort)SCProtocol.GetModuleInfoResponse, item));
        }

        public void SetSession(ISession session)
        {
            _session = session;
            _sessionContext = new SessionContext();
            _sessionContext.SetSession(_session);
            _session.AddSessionComponent(_sessionContext);
        }
    }
}
