using Dignus.Log;
using Dignus.Sockets.Interfaces;

namespace CommandSystem.Net.Components
{
    internal class AuthenticationComponent : ISessionComponent
    {
        private ISession _session;
        private readonly CancellationTokenSource _cts = new(10000);

        private SessionContext _sessionContext;
        private readonly TaskCompletionSource _taskCompletion = new TaskCompletionSource();
        public void Dispose()
        {
        }
        public void SetAuth()
        {
            _sessionContext.IsAuth = true;
            _taskCompletion.TrySetResult();
        }
        public async void SetSession(ISession session)
        {
            _session = session;

            foreach (var component in _session.GetSessionComponents())
            {
                if (component is SessionContext sessionContext)
                {
                    _sessionContext = sessionContext;
                    break;
                }
            }

            if (_sessionContext == null)
            {
                _session.Dispose();
                return;
            }
            try
            {
                await _taskCompletion.Task.WaitAsync(_cts.Token);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                _session.Dispose();
                return;
            }

            if (_sessionContext.IsAuth == false)
            {
                LogHelper.Error("authentication failed. Disposing session.");
                _session.Dispose();
                return;
            }
        }
    }
}
