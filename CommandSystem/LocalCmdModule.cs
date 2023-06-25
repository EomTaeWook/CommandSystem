using CommandSystem.Interface;
using CommandSystem.Models;
using Dignus.Log;
using System.Diagnostics;
using System.Text;

namespace CommandSystem
{
    public class LocalCmdModule : Module
    {
        protected CancellationTokenSource _cancellationToken = null;
        public LocalCmdModule(string moduleName = null) : base(moduleName)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;
        }
        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if(_cancellationToken == null)
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
        
        protected override void RunCommand(string line)
        {
            if(_cancellationToken != null)
            {
                LogHelper.Error("the command is currently in progress");
                return;
            }
            _cancellationToken = new CancellationTokenSource();
            RunCommand(line, false, _cancellationToken.Token).GetAwaiter().GetResult();
            
            Task.Run(() => 
            {
                _cancellationToken.Dispose();
                _cancellationToken = null;
                Prompt();
            });
        }
        protected Task RunCommand(string line, bool isAlias, CancellationToken cancellationToken)
        {
            var splits = line.Split(" ");
            return LocalCommandAsync(splits[0], splits[1..], isAlias, cancellationToken);
        }

        private async Task LocalCommandAsync(string command, string[] options, bool isAlias, CancellationToken cancellationToken)
        {
            var aliasTable = _builder.GetService<AliasTable>();
            if (aliasTable.Alias.ContainsKey(command) == true && isAlias == false)
            {
                var sb = new StringBuilder();
                sb.Append(aliasTable.Alias[command].Cmd);
                sb.Append(string.Join(" ", options));

                await RunCommand(sb.ToString(), true, cancellationToken);
            }
            ICmdProcessor cmdProcessor;
            try
            {
                cmdProcessor = _builder.GetService<ICmdProcessor>(command);
            }
            catch
            {
                LogHelper.Error($"not found command : {command}");
                return;
            }

            try
            {
                var task = Task.Run(async () => 
                {
                    try
                    {
                        await cmdProcessor.InvokeAsync(options, cancellationToken);
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                }, cancellationToken);
                await task.WaitAsync(cancellationToken);
            }
            catch(OperationCanceledException operationCanceledException)
            {
                LogHelper.Error($"failed to command : {operationCanceledException.Message}");
            }
            catch(Exception ex)
            {
                LogHelper.Error($"failed to command : {ex.Message}");
            }
        }

        public override void Run()
        {
            LogHelper.Info($"*** local module start ***");
            Prompt();
        }
    }
}
