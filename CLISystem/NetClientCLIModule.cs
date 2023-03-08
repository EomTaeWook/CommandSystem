using CLISystem.Net;

namespace CLISystem
{
    public class NetClientCLIModule : CLIModule
    {
        private readonly ClientModule _clientModule;
        public NetClientCLIModule(string moduleName = null) : base(moduleName)
        {
            _clientModule = new ClientModule();
        }
        protected override void RunCommnad(string line)
        {
            _clientModule.SendCommand(line);
        }
        public void Run(string ip, int port)
        {
            Task.Run(() =>
            {
                this.Run();
                _clientModule.Run(ip, port);
            }); 
        }
    }
}
