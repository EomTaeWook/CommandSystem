using CommandSystem.Extensions;
using CommandSystem.Interfaces;
using CommandSystem.Models;
using Dignus.DependencyInjection;
using Dignus.Log;
using System.Diagnostics;
using System.Text;

namespace CommandSystem
{
    public class LocalCmdModule : CommandProcessorBase
    {
        private CancellationTokenSource _cancellationToken = null;
        public LocalCmdModule(string moduleName = null) : this(moduleName, new ServiceContainer())
        {

        }
        internal LocalCmdModule(string moduleName, ServiceContainer commandServiceContainer) : base(moduleName, commandServiceContainer)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;
        }
        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (_cancellationToken == null)
            {
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                e.Cancel = true;
                Cancel();
            }
        }

        public void Cancel()
        {
            _cancellationToken.Cancel();
        }
        public override void RunCommand(string line)
        {
            if (_cancellationToken != null)
            {
                LogHelper.Error("the command is currently in progress");
                return;
            }
            _cancellationToken = new CancellationTokenSource();
            RunCommandAsync(line, false, _cancellationToken.Token).GetAwaiter().GetResult();
            _cancellationToken.Dispose();
            _cancellationToken = null;
            Task.Run(() =>
            {
                this.DisplayPrompt();
            });
        }
        public async Task RunCommandAsync(string line)
        {
            if (_cancellationToken != null)
            {
                LogHelper.Error("the command is currently in progress");
                return;
            }
            _cancellationToken = new CancellationTokenSource();
            await RunCommandAsync(line, false, _cancellationToken.Token);
            _cancellationToken.Dispose();
            _cancellationToken = null;
            _ = Task.Run(() =>
            {
                this.DisplayPrompt();
            });
        }
        public Task RunCommandAsync(string line, bool isAlias, CancellationToken cancellationToken)
        {
            var splits = line.Split(" ");
            return LocalCommandAsync(splits[0], splits[1..], isAlias, cancellationToken);
        }
        private async Task LocalCommandAsync(string command, string[] options, bool isAlias, CancellationToken cancellationToken)
        {
            var aliasTable = _commandServiceContainer.Resolve<AliasTable>();
            if (aliasTable.Alias.ContainsKey(command) == true && isAlias == false)
            {
                var sb = new StringBuilder();
                sb.Append(aliasTable.Alias[command].Cmd);
                sb.Append(string.Join(" ", options));

                await RunCommandAsync(sb.ToString(), true, cancellationToken);
            }
            ICommandAction cmdProcessor;
            try
            {
                cmdProcessor = _commandServiceContainer.Resolve<ICommandAction>(command);
            }
            catch
            {
                LogHelper.Error($"not found command : {command}");
                return;
            }
            try
            {
                await cmdProcessor.InvokeAsync(options, cancellationToken);
            }
            catch (OperationCanceledException operationCanceledException)
            {
                LogHelper.Error($"failed to command : {operationCanceledException.Message}");
            }
            catch (Exception ex)
            {
                LogHelper.Error($"failed to command : {ex.Message}");
            }
        }
        public void Run()
        {
            LogHelper.Info($"*** local module start ***");
            this.DisplayPrompt();
        }
    }
}
