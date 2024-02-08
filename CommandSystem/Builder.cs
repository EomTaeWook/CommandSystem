using CommandSystem.Attribute;
using CommandSystem.Interface;
using CommandSystem.Internal;
using CommandSystem.Models;
using Dignus.DependencyInjection;
using Dignus.DependencyInjection.Extensions;
using System.Reflection;
using System.Text.Json;

namespace CommandSystem
{
    internal class Builder
    {
        internal readonly CommandServiceContainer _commandContainer = new();
        private CommandTable _commandTable = new CommandTable();
        public Builder()
        {
            _commandContainer.RegisterType(_commandContainer);
            _commandContainer.RegisterType(_commandTable);
        }

        public void Build(Configuration configuration)
        {
            var assembly = Assembly.GetCallingAssembly();
            _commandContainer.RegisterDependencies(assembly);

            foreach (var item in assembly.GetTypes())
            {
                if (item.GetCustomAttribute<CmdAttribute>() != null || item.GetCustomAttribute<MultipleCmdAttribute>() != null || item.GetCustomAttribute<LocalCmdAttribute>() != null)
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
                _commandContainer.RegisterType(new AliasTable(new List<AliasModel>()));
            }
            _commandContainer.RegisterType(configuration);
            _commandContainer.Build();
        }
        public void AddProcessorType<T>(string commandName, T cmdProcessor, bool isLocal) where T : class, ICmdProcessor
        {
            if (isLocal == false)
            {
                _commandTable.AddCommand(commandName);
            }
            else
            {
                _commandTable.AddLoaclCommand(commandName);
            }

            _commandContainer.RegisterType(commandName, cmdProcessor);
        }

        public void AddProcessorType<T>(T cmdProcessor, bool isLocalCommand) where T : class, ICmdProcessor
        {
            var cmdAttributeType = typeof(CmdAttribute);
            var localCmdAttributeType = typeof(LocalCmdAttribute);
            var multipleCmdAttributeType = typeof(MultipleCmdAttribute);
            var processorType = cmdProcessor.GetType();

            if (processorType.IsDefined(cmdAttributeType) == true)
            {
                var attr = processorType.GetCustomAttribute(cmdAttributeType) as CmdAttribute;
                var name = attr.Name;
                AddProcessorType(name, cmdProcessor, isLocalCommand);
            }
            else if (processorType.IsDefined(multipleCmdAttributeType) == true)
            {
                var attr = processorType.GetCustomAttribute(multipleCmdAttributeType) as MultipleCmdAttribute;
                foreach (var name in attr.Names)
                {
                    AddProcessorType(name, cmdProcessor, isLocalCommand);
                }
            }
            else if (processorType.IsDefined(localCmdAttributeType))
            {
                var attr = processorType.GetCustomAttribute(localCmdAttributeType) as LocalCmdAttribute;
                AddProcessorType(attr.Name, cmdProcessor, isLocalCommand);
            }
            else
            {
                AddProcessorType(processorType.Name, cmdProcessor, isLocalCommand);
            }
        }
        public void AddProcessorType(Type type)
        {
            if (typeof(ICmdProcessor).IsAssignableFrom(type) == false || type.IsInterface == true)
            {
                throw new InvalidCastException(nameof(type));
            }

            var cmdAttributeType = typeof(CmdAttribute);
            var multipleCmdAttributeType = typeof(MultipleCmdAttribute);
            var localCmdAttributeType = typeof(LocalCmdAttribute);
            if (type.IsDefined(cmdAttributeType) == true)
            {
                var attr = type.GetCustomAttribute(cmdAttributeType) as CmdAttribute;
                _commandContainer.RegisterType(attr.Name, type, LifeScope.Transient);
                _commandTable.AddCommand(attr.Name);
            }
            else if (type.IsDefined(localCmdAttributeType))
            {
                var attr = type.GetCustomAttribute(localCmdAttributeType) as LocalCmdAttribute;
                _commandContainer.RegisterType(attr.Name, type, LifeScope.Transient);
                _commandTable.AddLoaclCommand(attr.Name);
            }
            else if (type.IsDefined(multipleCmdAttributeType) == true)
            {
                var attr = type.GetCustomAttribute(multipleCmdAttributeType) as MultipleCmdAttribute;
                foreach (var name in attr.Names)
                {
                    _commandContainer.RegisterType(name, type, LifeScope.Transient);
                    _commandTable.AddCommand(name);
                }
            }
            else
            {
                var name = type.Name;
                _commandTable.AddCommand(name);
                _commandContainer.RegisterType(name, type, LifeScope.Transient);
            }
        }
        public void AddProcessorType<T>() where T : class, ICmdProcessor
        {
            AddProcessorType(typeof(T));
        }
    }
}
