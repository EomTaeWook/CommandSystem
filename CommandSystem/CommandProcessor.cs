using CommandSystem.Attribute;
using CommandSystem.Cmd;
using CommandSystem.Interface;
using CommandSystem.Internal;
using CommandSystem.Models;
using Dignus.DependencyInjection;
using Dignus.DependencyInjection.Extensions;
using System.Reflection;
using System.Text.Json;

namespace CommandSystem
{
    public abstract class CommandProcessor : ICommandProcessor
    {
        internal readonly CommandServiceContainer _commandServiceContainer;
        private CommandTable _commandTable = new();
        private bool _isBuilt = false;
        internal string _moduleName;
        public abstract void RunCommand(string line);

        public CommandProcessor(string moduleName = null) : this(moduleName, new CommandServiceContainer())
        {
        }

        internal CommandProcessor(string moduleName, CommandServiceContainer commandServiceContainer)
        {
            if (string.IsNullOrEmpty(moduleName) == true)
            {
                moduleName = Assembly.GetEntryAssembly().GetName().Name;
            }
            _moduleName = moduleName;
            _commandServiceContainer = commandServiceContainer;
        }

        public void Build()
        {
            if (_isBuilt == true)
            {
                throw new InvalidOperationException("command module has already been built.");
            }
            _isBuilt = true;

            BuildInternal();
        }
        private void BuildInternal()
        {
            _commandServiceContainer.RegisterType(_commandServiceContainer);
            _commandServiceContainer.RegisterType(_commandTable);
            var assembly = Assembly.GetCallingAssembly();
            _commandServiceContainer.RegisterDependencies(assembly);
            foreach (var item in assembly.GetTypes())
            {
                if (item.GetCustomAttribute<CmdAttribute>() != null || item.GetCustomAttribute<MultipleCmdAttribute>() != null || item.GetCustomAttribute<LocalCmdAttribute>() != null)
                {
                    AddCommandAction(item);
                }
            }
            if (File.Exists(AliasTable.Path) == true)
            {
                var alias = JsonSerializer.Deserialize<List<AliasModel>>(File.ReadAllText(AliasTable.Path));
                _commandServiceContainer.RegisterType(new AliasTable(alias));
            }
            else
            {
                _commandServiceContainer.RegisterType(new AliasTable(new List<AliasModel>()));
            }
            _commandServiceContainer.Build();
        }
        public void AddCommandAction(string command, string desc, Func<string[], CancellationToken, Task> action, bool isLocalCommand = false)
        {
            AddCommandAction(command, new ActionCmd(action, desc), isLocalCommand);
        }
        public void AddCommandAction<T>(string commandName, T commandAction, bool isLocalCommand) where T : class, ICommandAction
        {
            if (isLocalCommand == false)
            {
                _commandTable.AddCommand(commandName);
            }
            else
            {
                _commandTable.AddLoaclCommand(commandName);
            }

            _commandServiceContainer.RegisterType(commandName, commandAction);
        }

        public void AddCommandAction<T>(T commandAction, bool isLocalCommand = false) where T : class, ICommandAction
        {
            var cmdAttributeType = typeof(CmdAttribute);
            var localCmdAttributeType = typeof(LocalCmdAttribute);
            var multipleCmdAttributeType = typeof(MultipleCmdAttribute);
            var processorType = commandAction.GetType();

            if (processorType.IsDefined(cmdAttributeType) == true)
            {
                var attr = processorType.GetCustomAttribute(cmdAttributeType) as CmdAttribute;
                var name = attr.Name;
                AddCommandAction(name, commandAction, isLocalCommand);
            }
            else if (processorType.IsDefined(multipleCmdAttributeType) == true)
            {
                var attr = processorType.GetCustomAttribute(multipleCmdAttributeType) as MultipleCmdAttribute;
                foreach (var name in attr.Names)
                {
                    AddCommandAction(name, commandAction, isLocalCommand);
                }
            }
            else if (processorType.IsDefined(localCmdAttributeType))
            {
                var attr = processorType.GetCustomAttribute(localCmdAttributeType) as LocalCmdAttribute;
                AddCommandAction(attr.Name, commandAction, isLocalCommand);
            }
            else
            {
                AddCommandAction(processorType.Name, commandAction, isLocalCommand);
            }
        }
        public void AddCommandAction<T>() where T : ICommandAction
        {
            AddCommandAction(typeof(T));
        }
        public void AddCommandAction(Type type)
        {
            if (typeof(ICommandAction).IsAssignableFrom(type) == false || type.IsInterface == true)
            {
                throw new InvalidCastException(nameof(type));
            }

            var cmdAttributeType = typeof(CmdAttribute);
            var multipleCmdAttributeType = typeof(MultipleCmdAttribute);
            var localCmdAttributeType = typeof(LocalCmdAttribute);
            if (type.IsDefined(cmdAttributeType) == true)
            {
                var attr = type.GetCustomAttribute(cmdAttributeType) as CmdAttribute;
                _commandServiceContainer.RegisterType(attr.Name, type, LifeScope.Transient);
                _commandTable.AddCommand(attr.Name);
            }
            else if (type.IsDefined(localCmdAttributeType))
            {
                var attr = type.GetCustomAttribute(localCmdAttributeType) as LocalCmdAttribute;
                _commandServiceContainer.RegisterType(attr.Name, type, LifeScope.Transient);
                _commandTable.AddLoaclCommand(attr.Name);
            }
            else if (type.IsDefined(multipleCmdAttributeType) == true)
            {
                var attr = type.GetCustomAttribute(multipleCmdAttributeType) as MultipleCmdAttribute;
                foreach (var name in attr.Names)
                {
                    _commandServiceContainer.RegisterType(name, type, LifeScope.Transient);
                    _commandTable.AddCommand(name);
                }
            }
            else
            {
                var name = type.Name;
                _commandTable.AddCommand(name);
                _commandServiceContainer.RegisterType(name, type, LifeScope.Transient);
            }
        }

        public string GetModuleName()
        {
            return _moduleName;
        }
    }
}
