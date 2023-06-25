using CommandSystem.ConsoleWriter;
using CommandSystem.Net;
using Dignus.Log;

namespace CommandSystem
{
    public class ServerCmdModule : LocalCmdModule
    {
        private readonly ServerModule _serverModule;
        private readonly int _port;
        public ServerCmdModule(int port, string moduleName = null) : base(moduleName)
        {
            _serverModule = new ServerModule(this);
            _port = port;
        }
        public void CacelCommand()
        {
            if(_cancellationToken != null)
            {
                _cancellationToken.Cancel();
            }
        }
        public async Task<string> ProcessCommandAsync(string line)
        {
            
            if (string.IsNullOrEmpty(line) == true)
            {
                LogHelper.Error("command is empty");
                return "command is empty";
            }
            if(_cancellationToken!= null)
            {
                LogHelper.Error("the command is currently in progress");
                return "the command is currently in progress";
            }
            var console = new RedirectConsoleWriter();
            _cancellationToken = new CancellationTokenSource();
            await RunCommand(line, false, _cancellationToken.Token);
            _cancellationToken.Dispose();
            _cancellationToken = null;
            var body = console.Release();
            var _ = Task.Run(Prompt);
            return body;
        }
        public override void Run()
        {
            RunAsync().GetAwaiter().GetResult();
        }
        public Task RunAsync()
        {
            return Task.Run(() =>
            {
                _serverModule.Run(_port, _builder);
                base.Run();
            });
        }
    }
}
