using Dignus.Sockets.Interfaces;

namespace CommandSystem.Net
{
    public class SessionContext : ISessionComponent
    {
        public bool IsAuth { get; set; }
        private ISession _session;
        public void Dispose()
        {
            _session = null;
        }

        public void SetSession(ISession session)
        {
            _session = session;
        }
        public void Send(IPacket packet)
        {
            if (_session == null)
            {
                return;
            }
            _session.Send(packet);
        }
    }
}
