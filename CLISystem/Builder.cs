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
        internal ProcessorNames _processorNames;
        public void Build(Configuration configuration)
        {
            var assembly = Assembly.GetCallingAssembly();

            _processorNames = new ProcessorNames(_serviceProvider);

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
            _serviceProvider.AddSingleton(_processorNames);
            _serviceProvider.AddSingleton(configuration);
            _serviceProvider.AddSingleton<ModuleConsoleWriter, ModuleConsoleWriter>();

            var _ = _serviceProvider.GetService<ModuleConsoleWriter>();
        }
        public void AddProcessorType<T>(T cmdProcessor) where T : class, ICmdProcessor
        {
            var cmdAttributeType = typeof(CmdAttribute);
            var multipleCmdAttributeType = typeof(MultipleCmdAttribute);
            var processorType = cmdProcessor.GetType();
            
            if (processorType.IsDefined(cmdAttributeType) == true)
            {
                var attr = processorType.GetCustomAttribute(cmdAttributeType) as CmdAttribute;
                var name = attr.Name;
                _serviceProvider.AddSingleton(name, cmdProcessor);
                _processorNames.Add(name);
            }
            else if(processorType.IsDefined(multipleCmdAttributeType) == true)
            {
                var attr = processorType.GetCustomAttribute(multipleCmdAttributeType) as MultipleCmdAttribute;
                foreach(var name in attr.Names)
                {
                    _serviceProvider.AddSingleton(name, cmdProcessor);
                    _processorNames.Add(name);
                }
            }
            else
            {
                var name = processorType.Name;
                _serviceProvider.AddSingleton(name, cmdProcessor);
                _processorNames.Add(name);
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

            if (type.IsDefined(cmdAttributeType) == true)
            {
                var attr = type.GetCustomAttribute(cmdAttributeType) as CmdAttribute;
                var name = attr.Name;
                _serviceProvider.AddTransient(name, type);
                _processorNames.Add(name);
            }
            else if (type.IsDefined(multipleCmdAttributeType) == true)
            {
                var attr = type.GetCustomAttribute(multipleCmdAttributeType) as MultipleCmdAttribute;
                foreach (var name in attr.Names)
                {
                    _serviceProvider.AddTransient(name, type);
                    _processorNames.Add(name);
                }
            }
            else
            {
                var name = type.Name;
                _serviceProvider.AddTransient(name, type);
                _processorNames.Add(name);
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
