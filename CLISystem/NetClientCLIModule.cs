using CLISystem.Net;

namespace CLISystem
{
    public class NetClientCLIModule : CLIModule
    {
        public static NetClientCLIModule Instance;
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
            this.Run();
            _clientModule.Run(ip, port);
            Instance = this;
        }
    }
}
