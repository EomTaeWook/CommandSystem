using CLISystem.ConsoleWriter;
using CLISystem.Interface;
using CLISystem.Models;
using Kosher.Log;
using System.Reflection;
using System.Text;

namespace CLISystem
{
    public class CLIModule
    {        
        internal readonly Builder _builder = new();

        private bool _isBuild = false;
        private readonly Configuration _configuration = new Configuration();
        public CLIModule(string moduleName = null)
        {
            if(string.IsNullOrEmpty(moduleName) == true)
            {
                moduleName = Assembly.GetEntryAssembly().GetName().Name;
            }
            _configuration.ModuleName = moduleName;
        }
        protected virtual void RunCommnad(string line)
        {
            var splits = line.Split(" ");
            LocalCommand(splits[0], splits[1..]);
        }
        private void LocalCommand(string command, string[] options)
        {
            var aliasTable = _builder.GetService<AliasTable>();
            if (aliasTable.Alias.ContainsKey(command) == true)
            {
                var sb = new StringBuilder();
                sb.Append(aliasTable.Alias[command].Cmd);
                sb.Append(string.Join(" ", options));

                RunCommnad(sb.ToString());
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
            catch(Exception ex)
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
        public void Build(Configuration configuration = null)
        {
            if(_isBuild == true)
            {
                throw new InvalidOperationException("already build cli module");
            }
            _isBuild = true;

            if(configuration == null)
            {
                configuration = _configuration;
            }
            _builder.Build(configuration);
        }
        public void Run()
        {
            Console.WriteLine($"*** cli module start ***");
            var writer = _builder.GetService<ModuleConsoleWriter>();
            while (true)
            {
                writer.Refresh();
                var line = Console.ReadLine();
                if (line.Length == 0)
                {
                    continue;
                }
                RunCommnad(line);
            }
        }
    }
}
