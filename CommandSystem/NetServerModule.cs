using CommandSystem.ConsoleWriter;
using CommandSystem.Net;

namespace CommandSystem
{
    public class NetServerModule : Module
    {
        private readonly ServerModule _serverModule;
        private readonly int _port;
        public NetServerModule(int port, string moduleName = null) : base(moduleName)
        {
            _serverModule = new ServerModule();
            _port = port;
        }
        public bool ProcessCommnad(string line, out string body)
        {
            body = string.Empty;
            if (string.IsNullOrEmpty(line) == true)
            {
                return false;
            }
            var console = new RedirectConsoleWriter();

            RunCommand(line);

            body = console.Release();

            return true;
        }
        public override void Run()
        {
            RunAsync().GetAwaiter().GetResult();
        }
        public Task RunAsync()
        {
            return Task.Run(() =>
            {
                _serverModule.SetNetCLIModule(this);
                _serverModule.Run(_port, _builder);
                base.Run();
            });
        }
    }
}
