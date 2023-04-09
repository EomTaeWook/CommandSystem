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
        internal ServiceProvider _serviceProvider = new();
        public void Build(Configuration configuration)
        {
            _serviceProvider.AddSingleton(new HashSet<string>());

            var assembly = Assembly.GetCallingAssembly();

            foreach (var item in assembly.GetTypes())
            {
                if(typeof(ICmdProcessor).IsAssignableFrom(item) && item.IsInterface == false)
                {
                    AddProcessorType(item);
                }
            }
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
            _serviceProvider.AddSingleton(_serviceProvider);

            var _ = _serviceProvider.GetService<ModuleConsoleWriter>();
        }
        public void AddProcessorType<T>(T cmdProcessor) where T : class, ICmdProcessor
        {
            var cmdAttributeType = typeof(CmdAttribute);
            var multipleCmdAttributeType = typeof(MultipleCmdAttribute);
            var processorType = cmdProcessor.GetType();

            var cmdToMap = _serviceProvider.GetService<HashSet<string>>();

            if (processorType.IsDefined(cmdAttributeType) == true)
            {
                var attr = processorType.GetCustomAttribute(cmdAttributeType) as CmdAttribute;
                var name = attr.Name;
                _serviceProvider.AddSingleton(name, cmdProcessor);

                cmdToMap.Add(name);
            }
            else if(processorType.IsDefined(multipleCmdAttributeType) == true)
            {
                var attr = processorType.GetCustomAttribute(multipleCmdAttributeType) as MultipleCmdAttribute;
                foreach(var name in attr.Names)
                {
                    cmdToMap.Add(name);
                    _serviceProvider.AddSingleton(name, cmdProcessor);
                }
            }
            else
            {
                var name = processorType.Name;
                cmdToMap.Add(name);
                _serviceProvider.AddSingleton(name, cmdProcessor);
            }
        }
        public void AddProcessorType(Type type)
        {
            if(typeof(ICmdProcessor).IsAssignableFrom(type) == false || type.IsInterface == true)
            {
                throw new InvalidCastException(nameof(type));
            }
            var cmdAttributeType = typeof(CmdAttribute);
            var multipleCmdAttributeType = typeof(MultipleCmdAttribute);
            var cmdToMap = _serviceProvider.GetService<HashSet<string>>();
            if (type.IsDefined(cmdAttributeType) == true)
            {
                var attr = type.GetCustomAttribute(cmdAttributeType) as CmdAttribute;
                var name = attr.Name;
                _serviceProvider.AddTransient(name, type);
                cmdToMap.Add(name);
            }
            else if (type.IsDefined(multipleCmdAttributeType) == true)
            {
                var attr = type.GetCustomAttribute(multipleCmdAttributeType) as MultipleCmdAttribute;
                foreach (var name in attr.Names)
                {
                    _serviceProvider.AddTransient(name, type);
                    cmdToMap.Add(name);
                }
            }
            else
            {
                var name = type.Name;
                cmdToMap.Add(name);
                _serviceProvider.AddTransient(name, type);
            }
        }
        public void AddProcessorType<T>() where T : class, ICmdProcessor
        {
            AddProcessorType(typeof(T));
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
