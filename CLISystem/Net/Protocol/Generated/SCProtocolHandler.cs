using Kosher.Log;

namespace CLISystem.Net.Protocol.Handler
{
    public interface ISCProtocolHandler
    {
        public T DeserializeBody<T>(string body);
    }
    public partial class SCProtocolHandler : ISCProtocolHandler
    {
        private static Action<SCProtocolHandler, string>[] _handlers;
        public static void Init()
        {
            _handlers = new Action<SCProtocolHandler, string>[2];
            _handlers[0] = (t, body) => t.ProcessGetModuleInfoResponse(body);
            _handlers[1] = (t, body) => t.ProcessRemoteCommandResponse(body);
        }
        public static bool CheckProtocol(int protocol)
        {
            if(protocol < 0 && protocol >= _handlers.Length)
            {
                return false;
            }
            return true;
        }
        public void Process(int protocol, string body)
        {
            _handlers[protocol](this, body);
        }
        protected void ProcessGetModuleInfoResponse(string body)
        {
            if(body == null)
            {
                LogHelper.Error("body is null");
                return;
            }
            var packet = DeserializeBody<CLISystem.Net.Protocol.Models.GetModuleInfoResponse>(body);
            Process(packet);
        }
        protected void ProcessRemoteCommandResponse(string body)
        {
            if(body == null)
            {
                LogHelper.Error("body is null");
                return;
            }
            var packet = DeserializeBody<CLISystem.Net.Protocol.Models.RemoteCommandResponse>(body);
            Process(packet);
        }
    }
}
