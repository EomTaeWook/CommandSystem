using CommandSystem.Cmd;
using CommandSystem.ConsoleWriter;
using CommandSystem.Interface;
using CommandSystem.Models;
using Dignus.Log;
using System.Reflection;
using System.Text;

namespace CommandSystem
{
    public class Module
    {
        internal Builder _builder = new();

        private bool _isBuild = false;
        private readonly Configuration _configuration = new();
        public Module(string moduleName = null)
        {
            if (string.IsNullOrEmpty(moduleName) == true)
            {
                moduleName = Assembly.GetEntryAssembly().GetName().Name;
            }
            _configuration.ModuleName = moduleName;
        }
        private void RunCommnad(string line, bool isAlias = false)
        {
            var splits = line.Split(" ");
            LocalCommand(splits[0], splits[1..], isAlias);
        }
        protected virtual void RunCommnad(string line)
        {
            RunCommnad(line, false);
        }
        private void LocalCommand(string command, string[] options, bool isAlias)
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
                cmdProcessor.InvokeAsync(options).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                LogHelper.Error($"failed to command : {ex.Message}");
            }
        }

        public void AddCmdProcessor<T>(T processor) where T : class, ICmdProcessor
        {
            _builder.AddProcessorType(processor);
        }
        public void AddCmdProcessor<T>() where T : class, ICmdProcessor
        {
            _builder.AddProcessorType<T>();
        }
        public void AddCmdProcessor(string command, string desc, Action action)
        {
            Action<string[]> cmdAction = (string[] args) =>
            {
                action?.Invoke();
            };
            AddCmdProcessor(command, desc, cmdAction);
        }
        public void AddCmdProcessor(string command, string desc, Action<string[]> action)
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
            while (true)
            {
                Prompt();
                var line = Console.ReadLine();
                if (line.Length == 0)
                {
                    continue;
                }
                RunCommnad(line);
            }
        }
        private void Prompt()
        {
            Console.Write($"{_configuration.ModuleName} > ");
        }
    }
}
