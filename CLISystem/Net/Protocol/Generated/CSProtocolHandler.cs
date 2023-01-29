using Kosher.Log;

namespace CLISystem.Net.Protocol.Handler
{
    public interface ICSProtocolHandler
    {
        public T DeserializeBody<T>(string body);
    }
    public partial class CSProtocolHandler : ICSProtocolHandler
    {
        private static Action<CSProtocolHandler, string>[] _handlers;
        public static void Init()
        {
            _handlers = new Action<CSProtocolHandler, string>[2];
            _handlers[0] = (t, body) => t.ProcessGetModuleInfo(body);
            _handlers[1] = (t, body) => t.ProcessRemoteCommand(body);
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
        protected void ProcessGetModuleInfo(string body)
        {
            if(body == null)
            {
                LogHelper.Error("body is null");
                return;
            }
            var packet = DeserializeBody<CLISystem.Net.Protocol.Models.GetModuleInfo>(body);
            Process(packet);
        }
        protected void ProcessRemoteCommand(string body)
        {
            if(body == null)
            {
                LogHelper.Error("body is null");
                return;
            }
            var packet = DeserializeBody<CLISystem.Net.Protocol.Models.RemoteCommand>(body);
            Process(packet);
        }
    }
}
