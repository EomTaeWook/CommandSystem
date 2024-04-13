using CommandSystem.Extensions;
using CommandSystem.Interface;
using CommandSystem.Models;
using CommandSystem.Net;
using Dignus.Log;
using System.Diagnostics;

namespace CommandSystem
{
    public class ClientCmdModule : CommandProcessorBase
    {
        private readonly ClientModule _clientModule;
        private readonly string _ip;
        private readonly int _port;
        public string IpString { get => _ip; }
        public int JobId { get; private set; } = -1;

        private CancellationTokenSource _localCancellationToken;
        public ClientCmdModule(string ip, int port, string moduleName = null) : base(moduleName)
        {
            _clientModule = new ClientModule(this);
            _ip = ip;
            _port = port;
            Console.CancelKeyPress += Console_CancelKeyPress;
        }
        public void ResetJobId()
        {
            JobId = -1;
        }
        public void SetJobId(int jobId)
        {
            JobId = jobId;
        }
        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (JobId != -1)
            {
                e.Cancel = true;
                _clientModule.CacelCommand(JobId);
            }
            else
            {
                Process.GetCurrentProcess().Kill();
            }
        }
        private async Task<bool> ProcessLocalCommandAsync(string command, string[] options, bool isAlias, CancellationToken cancellationToken)
        {
            if (isAlias == false)
            {
                var table = _commandServiceContainer.Resolve<AliasTable>();
                if (table.Alias.TryGetValue(command, out AliasModel alias) == true)
                {
                    return await ProcessLocalCommandAsync(alias.Cmd, options, true, cancellationToken);
                }
            }

            var commandTable = _commandServiceContainer.Resolve<CommandTable>();

            if (!commandTable.IsContainLocalCommand(command))
            {
                return false;
            }
            try
            {
                var cmdProcessor = _commandServiceContainer.Resolve<ICommandAction>(command);
                await cmdProcessor.InvokeAsync(options, cancellationToken);
                return true;
            }
            catch (OperationCanceledException operationCanceledException)
            {
                LogHelper.Error($"failed to command : {operationCanceledException.Message}");
            }
            catch (Exception ex)
            {
                LogHelper.Error($"failed to command : {ex.Message}");
            }
            return false;
        }
        public override void RunCommand(string line)
        {
            if (_localCancellationToken != null)
            {
                LogHelper.Error("the command is currently in progress");
                return;
            }

            var splits = line.Split(" ");

            _localCancellationToken = new CancellationTokenSource();
            var result = ProcessLocalCommandAsync(splits[0], splits[1..], false, _localCancellationToken.Token).Result;

            _localCancellationToken.Dispose();
            _localCancellationToken = null;

            if (result == true)
            {
                this.DisplayPrompt();
                return;
            }

            _clientModule.SendCommand(line);
        }
        public void Run()
        {
            RunAsync().GetAwaiter().GetResult();
        }
        internal void SetModuleName(string moduleName)
        {
            _moduleName = moduleName;
        }
        private Task RunAsync()
        {
            return Task.Run(() =>
            {
                _clientModule.Run(_ip, _port);
            });
        }
    }
}
