using CLISystem.Attribude;
using CLISystem.ConsoleWriter;
using CLISystem.Interface;
using CLISystem.Models;
using Kosher.Framework;
using System.Reflection;
using System.Text.Json;

namespace CLISystem
{
    internal class Builder
    {
        internal IList<string> _processTypeNames = new List<string>();
        internal ServiceProvider _serviceProvider = new();

        public void Build(Configuration configuration)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var cmdAttribudeType = typeof(CmdAttribute);

            foreach (var item in assembly.GetTypes())
            {
                if (item.IsDefined(cmdAttribudeType))
                {
                    var attr = item.GetCustomAttribute(cmdAttribudeType) as CmdAttribute;
                    AddNamedCmdType(attr.Name, item);
                    _processTypeNames.Add(attr.Name);
                }
            }
            _processTypeNames = _processTypeNames.AsReadOnly();
            if(File.Exists(AliasTable.Path) == true)
            {
                var alias = JsonSerializer.Deserialize<List<AliasModel>>(File.ReadAllText(AliasTable.Path));
                _serviceProvider.AddSingleton(new AliasTable(alias));
            }
            else
            {
                _serviceProvider.AddSingleton(new AliasTable());
            }

            _serviceProvider.AddSingleton(configuration);
            _serviceProvider.AddSingleton<ModuleConsoleWriter, ModuleConsoleWriter>();

            var _ = _serviceProvider.GetService<ModuleConsoleWriter>();
        }
        public void AddProcessorType<T>(T cmdProcessor) where T : class, ICmdProcessor
        {
            var cmdAttribudeType = typeof(CmdAttribute);
            var processorType = cmdProcessor.GetType();
            string name;
            if (processorType.IsDefined(cmdAttribudeType) == true)
            {
                var attr = processorType.GetCustomAttribute(cmdAttribudeType) as CmdAttribute;
                name = attr.Name;
            }
            else
            {
                name = processorType.Name;
            }
            _serviceProvider.AddSingleton(name, cmdProcessor);
            _processTypeNames.Add(name);
        }
        public void AddProcessorType<T>() where T : class, ICmdProcessor
        {
            var cmdAttribudeType = typeof(CmdAttribute);
            var processorType = typeof(T);
            string name;
            if (processorType.IsDefined(cmdAttribudeType) == true)
            {
                var attr = processorType.GetCustomAttribute(cmdAttribudeType) as CmdAttribute;
                name = attr.Name;
            }
            else
            {
                name = processorType.Name;
            }
            AddNamedCmdType(name, processorType);
            _processTypeNames.Add(name);
        }

        private void AddNamedCmdType(string typeName, Type type)
        {
            _serviceProvider.AddTransient(typeName, type);
        }
        public void AddSingletonService<T>(T implementation) where T : class
        {
            _serviceProvider.AddSingleton(implementation);
        }
        public T GetService<T>(string typeName) where T : class
        {
            return _serviceProvider.GetService<T>(typeName);
        }
        public T GetService<T>() where T : class
        {
            return _serviceProvider.GetService<T>();
        }
    }
}
