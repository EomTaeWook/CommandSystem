using CommandSystem.Cmd;
using CommandSystem.Interface;
using CommandSystem.Models;
using Dignus.Log;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace CommandSystem
{
    public class Module
    {
        internal Builder _builder = new();

        private bool _isBuild = false;
        private readonly Configuration _configuration = new();
        private CancellationTokenSource _cancellationToken = null;
        public Module(string moduleName = null)
        {
            if (string.IsNullOrEmpty(moduleName) == true)
            {
                moduleName = Assembly.GetEntryAssembly().GetName().Name;
            }
            _configuration.ModuleName = moduleName;
            Console.CancelKeyPress += Console_CancelKeyPress; ;
        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (_cancellationToken != null)
            {
                _cancellationToken.Cancel();
            }
            else
            {
                Process.GetCurrentProcess().Kill();
            }
        }
        private void RunCommnad(string line, bool isAlias = false)
        {
            var splits = line.Split(" ");
            var _ = LocalCommandAsync(splits[0], splits[1..], isAlias);
        }
        protected virtual void RunCommand(string line)
        {
            RunCommnad(line, false);
        }
        private async Task LocalCommandAsync(string command, string[] options, bool isAlias)
        {
            var aliasTable = _builder.GetService<AliasTable>();
            if (aliasTable.Alias.ContainsKey(command) == true && isAlias == false)
            {
                var sb = new StringBuilder();
                sb.Append(aliasTable.Alias[command].Cmd);
                sb.Append(string.Join(" ", options));

                RunCommnad(sb.ToString(), true);
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
                _cancellationToken = new CancellationTokenSource();
                var task = Task.Run(async () => 
                {
                    try
                    {
                        await cmdProcessor.InvokeAsync(options, _cancellationToken.Token);
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                });
                await task.WaitAsync(_cancellationToken.Token);
            }
            catch(AggregateException aggregateException)
            {
                LogHelper.Error($"failed to command : {aggregateException.Message}");
            }
            catch(OperationCanceledException operationCanceledException)
            {
                LogHelper.Error($"failed to command : {operationCanceledException.Message}");
            }
            catch(Exception ex)
            {
                LogHelper.Error($"failed to command : {ex.Message}");
            }
            finally
            {
                _cancellationToken.Dispose();
                _cancellationToken = null;
            }
            Prompt();
        }

        public void AddCmdProcessor<T>(T processor) where T : class, ICmdProcessor
        {
            _builder.AddProcessorType(processor);
        }
        public void AddCmdProcessor<T>() where T : class, ICmdProcessor
        {
            _builder.AddProcessorType<T>();
        }
        public void AddCmdProcessor(string command, string desc, Func<string[], CancellationToken, Task> action)
        {
            _builder.AddProcessorType(command, new ActionCmd(action, desc));
        }
        public void Build(Configuration configuration = null)
        {
            if (_isBuild == true)
            {
                throw new InvalidOperationException("already build cli module");
            }
            _isBuild = true;

            if (configuration == null)
            {
                configuration = _configuration;
            }
            _builder.Build(configuration);
        }
        public virtual void Run()
        {
            Console.WriteLine($"*** cli module start ***");
            Prompt();
        }
        private void Prompt()
        {
            Console.Write($"{_configuration.ModuleName} > ");
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                Task.Run(Prompt);
                return;
            }
            RunCommand(line);
        }
    }
}
