using CommandSystem.Net;
using Dignus.Log;
using System.Diagnostics;

namespace CommandSystem
{
    public class ClientCmdModule : Module
    {
        private readonly ClientModule _clientModule;
        private readonly string _ip;
        private readonly int _port;

        public bool IsRequested { get; set; }
        public ClientCmdModule(string ip, int port, string moduleName = null) : base(moduleName)
        {
            _clientModule = new ClientModule(this);
            _ip = ip;
            _port = port;
            Console.CancelKeyPress += Console_CancelKeyPress;
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (IsRequested == true)
            {
                e.Cancel = true;
                _clientModule.CacelCommand();
            }
            else
            {
                //Process.GetCurrentProcess().Kill();
            }
        }

        protected override void RunCommand(string line)
        {
            _clientModule.SendCommand(line);
            IsRequested = true;
        }
        public override void Run()
        {
            RunAsync().GetAwaiter().GetResult();
        }
        private Task RunAsync()
        {
            return Task.Run(() =>
            {
                _clientModule.Run(_ip, _port);

                LogHelper.Info($"*** cli client module start ***");
                Prompt();
            });
        }
    }
}
