using CLISystem.ConsoleWriter;
using CLISystem.Net;

namespace CLISystem
{
    public class NetCLIModule : CLIModule
    {
        private readonly ServerModule _serverModule;
        public NetCLIModule(string moduleName = null) : base(moduleName)
        {
            _serverModule = new ServerModule();
        }
        public bool ProcessCommnad(string line, out string body)
        {
            body = string.Empty;
            if(string.IsNullOrEmpty(line) == true)
            {
                return false;
            }
            var console = new RedirectConsoleWriter();

            this.RunCommnad(line);

            body = console.Release();

            return true;
        }

        public void Run(int port)
        {
            Task.Run(() => {
                this.Run();
                _serverModule.SetNetCLIModule(this);
                _serverModule.Run(port, this._builder);
            });
        }
    }
}
