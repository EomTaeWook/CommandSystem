using CommandSystem.Attribude;
using CommandSystem.Interface;
using CommandSystem.Internal;
using CommandSystem.Models;
using System.Reflection;
using System.Text.Json;

namespace CommandSystem
{
    internal class Builder
    {
        internal readonly CommandServiceContainer _commandContainer = new();

        public Builder()
        {
            _commandContainer.RegisterType(_commandContainer);
            _commandContainer.RegisterType(new CommandTable());
        }

        public void Build(Configuration configuration)
        {
            var assembly = Assembly.GetCallingAssembly();
            foreach (var item in assembly.GetTypes())
            {
                var cmdAttr = item.GetCustomAttribute<CmdAttribute>();
                var multiCmdAttr = item.GetCustomAttribute<MultipleCmdAttribute>();
                if (cmdAttr != null || multiCmdAttr != null)
                {
                    AddProcessorType(item);
                }
            }
            if (File.Exists(AliasTable.Path) == true)
            {
                var alias = JsonSerializer.Deserialize<List<AliasModel>>(File.ReadAllText(AliasTable.Path));
                _commandContainer.RegisterType(new AliasTable(alias));
            }
            else
            {
                _commandContainer.RegisterType(new AliasTable());
            }
            _commandContainer.RegisterType(configuration);
        }
        public void AddProcessorType<T>(string commandName, T cmdProcessor) where T : class, ICmdProcessor
        {
            var cmdToMap = _commandContainer.Resolve<CommandTable>();
            cmdToMap.AddCommand(commandName);
            _commandContainer.RegisterType(commandName, cmdProcessor);
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
            var cmdToMap = _commandContainer.Resolve<CommandTable>();

            var cmdAttributeType = typeof(CmdAttribute);
            var multipleCmdAttributeType = typeof(MultipleCmdAttribute);
            if (type.IsDefined(cmdAttributeType) == true)
            {
                var attr = type.GetCustomAttribute(cmdAttributeType) as CmdAttribute;
                var name = attr.Name;
                _commandContainer.RegisterType(name, type, Dignus.DependencyInjection.LifeScope.Transient);
                cmdToMap.AddCommand(name);
            }
            else if (type.IsDefined(multipleCmdAttributeType) == true)
            {
                var attr = type.GetCustomAttribute(multipleCmdAttributeType) as MultipleCmdAttribute;
                foreach (var name in attr.Names)
                {
                    _commandContainer.RegisterType(name, type, Dignus.DependencyInjection.LifeScope.Transient);
                    cmdToMap.AddCommand(name);
                }
            }
            else
            {
                var name = type.Name;
                cmdToMap.AddCommand(name);
                _commandContainer.RegisterType(name, type, Dignus.DependencyInjection.LifeScope.Transient);
            }
        }
        public void AddProcessorType<T>() where T : class, ICmdProcessor
        {
            AddProcessorType(typeof(T));
        }
    }
}
