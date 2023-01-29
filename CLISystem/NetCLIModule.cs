using CLISystem.ConsoleWriter;
using CLISystem.Net;

namespace CLISystem
{
    public class NetCLIModule : CLIModule
    {
        public static NetCLIModule Instance;
        private ServerModule _serverModule;
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
            this.Run();
            _serverModule.Run(port, this._builder);
            Instance = this;
        }
    }
}
