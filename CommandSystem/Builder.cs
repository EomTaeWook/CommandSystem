using CommandSystem.Attribude;
using CommandSystem.Interface;
using CommandSystem.Models;
using Dignus.DependencyInjection;
using Dignus.Log;
using Dignus.Log.LogTarget;
using Dignus.Log.Model.Rule;
using System.Reflection;
using System.Text.Json;

namespace CommandSystem
{
    internal class Builder
    {
        private readonly ServiceProvider _serviceProvider = new();
        
        public Builder() 
        {
            _serviceProvider.AddSingleton(new HashSet<string>());
            _serviceProvider.AddSingleton(_serviceProvider);
        }

        public void Build(Configuration configuration)
        {
            var assembly = Assembly.GetCallingAssembly();

            foreach (var item in assembly.GetTypes())
            {
                var cmdAttr = item.GetCustomAttribute<CmdAttribute>();
                var multiCmdAttr = item.GetCustomAttribute<MultipleCmdAttribute>();
                if(cmdAttr != null || multiCmdAttr != null)
                {
                    AddProcessorType(item);
                }
            }
            if (File.Exists(AliasTable.Path) == true)
            {
                var alias = JsonSerializer.Deserialize<List<AliasModel>>(File.ReadAllText(AliasTable.Path));
                _serviceProvider.AddSingleton(new AliasTable(alias));
            }
            else
            {
                _serviceProvider.AddSingleton(new AliasTable());
            }
            _serviceProvider.AddSingleton(configuration);
        }
        public void AddProcessorType<T>(string commandName, T cmdProcessor) where T : class, ICmdProcessor 
        {
            var cmdToMap = _serviceProvider.GetService<HashSet<string>>();
            cmdToMap.Add(commandName);
            _serviceProvider.AddSingleton(commandName, cmdProcessor);
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
                AddProcessorType(name, cmdProcessor);
            }
            else if (processorType.IsDefined(multipleCmdAttributeType) == true)
            {
                var attr = processorType.GetCustomAttribute(multipleCmdAttributeType) as MultipleCmdAttribute;
                foreach (var name in attr.Names)
                {
                    AddProcessorType(name, cmdProcessor);
                }
            }
            else
            {
                AddProcessorType(processorType.Name, cmdProcessor);
            }
        }
        public void AddProcessorType(Type type)
        {
            if (typeof(ICmdProcessor).IsAssignableFrom(type) == false || type.IsInterface == true)
            {
                throw new InvalidCastException(nameof(type));
            }
            var cmdToMap = _serviceProvider.GetService<HashSet<string>>();

            var cmdAttributeType = typeof(CmdAttribute);
            var multipleCmdAttributeType = typeof(MultipleCmdAttribute);
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
