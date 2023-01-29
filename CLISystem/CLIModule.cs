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
        private readonly Dictionary<string, ICmdProcessor> _cmdProcessor = new();
        private bool _isBuild = false;
        private Configuration _configuration = new Configuration();
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
                sb.Append(String.Join(" ", options));

                RunCommnad(sb.ToString());
            }
            if (_cmdProcessor.ContainsKey(command) == false)
            {
                LogHelper.Error($"not found command : {command}");
                return;
            }

            var cmdProcessor = _builder.GetService<ICmdProcessor>(command);
            try
            {    
                cmdProcessor.Invoke(options);
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
            _builder.AddSingletonService(_cmdProcessor);
            if(configuration == null)
            {
                configuration = _configuration;
            }
            _builder.Build(configuration);

            foreach(var item in _builder._processTypeNames)
            {
                var processor = _builder.GetService<ICmdProcessor>(item);
                _cmdProcessor.Add(item, processor);
            }
        }
        public void Run()
        {
            Task.Run(() => 
            {
                Console.WriteLine($"*** cli module start ***");

                while (true)
                {
                    var line = Console.ReadLine();
                    if (line.Length == 0)
                    {
                        continue;
                    }
                    RunCommnad(line);
                }
            });
        }
    }
}
