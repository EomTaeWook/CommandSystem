﻿using CLISystem.Models;
using CLISystem.Net.Protocol;
using CLISystem.Net.Protocol.Models;
using Kosher.Log;
using Kosher.Sockets;
using Kosher.Sockets.Attribute;
using Kosher.Sockets.Interface;
using System.Text.Json;

namespace CLISystem.Net.Handler
{
    public class CSProtocolHandler : ISessionComponent, IProtocolHandler<string>
    {
        private NetCLIModule _netCLIModule;
        public CSProtocolHandler(NetCLIModule netCLIModule)
        {
            _netCLIModule = netCLIModule;
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
        [ProtocolName("RemoteCommand")]
        public void Process(RemoteCommand remoteCommand)
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
                _netCLIModule.ProcessCommnad(remoteCommand.Cmd, out string body);
                var packet = Packet.MakePacket((ushort)SCProtocol.RemoteCommandResponse,
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
            var config = _netCLIModule._builder.GetService<Configuration>();
            var item = new GetModuleInfoResponse()
            {
                ModuleName = config.ModuleName
            };
            Session.Send(Packet.MakePacket((ushort)SCProtocol.GetModuleInfoResponse, item));
        }
    }
}