using CommandSystem.Net;

namespace CommandSystem
{
    public class NetClientModule : Module
    {
        private readonly ClientModule _clientModule;
        private readonly string _ip;
        private readonly int _port;

        public NetClientModule(string ip, int port, string moduleName = null) : base(moduleName)
        {
            _clientModule = new ClientModule();
            _ip = ip;
            _port = port;
        }
        protected override void RunCommnad(string line)
        {
            _clientModule.SendCommand(line);
        }
        public override void Run()
        {
            RunAsync().GetAwaiter().GetResult();
        }
        public Task RunAsync()
        {
            return Task.Run(() =>
            {
                _clientModule.Run(_ip, _port);
                base.Run();
            });
        }
    }
}
