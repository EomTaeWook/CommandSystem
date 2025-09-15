using CommandSystem.Attribute;
using CommandSystem.Cmd;
using CommandSystem.Interfaces;
using CommandSystem.Internals;
using CommandSystem.Models;
using Dignus.DependencyInjection;
using Dignus.DependencyInjection.Extensions;
using System.Reflection;
using System.Text.Json;

namespace CommandSystem
{
    public abstract class CommandProcessorBase : ICommandProcessor
    {
        internal readonly ServiceContainer _commandServiceContainer;
        private CommandTable _commandTable = new();
        private bool _isBuilt = false;
        internal string _moduleName;
        public abstract void RunCommand(string line);

        public CommandProcessorBase(string moduleName = null) : this(moduleName, new ServiceContainer())
        {
        }

        internal CommandProcessorBase(string moduleName, ServiceContainer commandServiceContainer)
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
        private void RegisterCommandActions(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes<CommandAttribute>().Any() ||
                    type.GetCustomAttributes<MultipleCmdAttribute>().Any() ||
                    type.GetCustomAttributes<LocalCmdAttribute>().Any())
                {
                    AddCommandAction(type);
                }
            }
        }
        private void BuildInternal()
        {
            _commandServiceContainer.RegisterType(_commandServiceContainer);
            _commandServiceContainer.RegisterType(_commandTable);
            var assembly = Assembly.GetCallingAssembly();
            _commandServiceContainer.RegisterDependencies(assembly);
            RegisterCommandActions(assembly);

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

        public void AddCommandAction(string command, string desc, Func<string[], CancellationToken, Task> action)
        {
            AddCommandAction(command, new ActionCmd(action, desc), false);
        }
        private void AddCommandAction<T>(string commandName, T commandAction, bool isLocalCommand) where T : class, ICommandAction
        {
            if (isLocalCommand == true)
            {
                _commandTable.AddLoaclCommand(commandName);
            }
            else
            {
                _commandTable.AddCommand(commandName);
            }
            _commandServiceContainer.RegisterType(commandName, commandAction);
        }

        public void AddCommandAction<T>(T commandAction) where T : class, ICommandAction
        {
            var commandType = commandAction.GetType();
            var cmdAttributeType = typeof(CommandAttribute);
            var multipleCmdAttributeType = typeof(MultipleCmdAttribute);
            var localCmdAttributeType = typeof(LocalCmdAttribute);

            if (commandType.IsDefined(localCmdAttributeType))
            {
                var attr = commandType.GetCustomAttribute<LocalCmdAttribute>();
                AddCommandAction(attr.Name, commandAction, true);
            }
            else if (commandType.IsDefined(multipleCmdAttributeType))
            {
                var attr = commandType.GetCustomAttribute<MultipleCmdAttribute>();
                foreach (var item in attr.Names)
                {
                    AddCommandAction(item, commandAction, false);
                }
            }
            else if (commandType.IsDefined(cmdAttributeType))
            {
                var attr = commandType.GetCustomAttribute<CommandAttribute>();
                AddCommandAction(attr.CommandName, commandAction, false);
            }
        }
        public void AddCommandAction<T>() where T : ICommandAction
        {
            AddCommandAction(typeof(T));
        }
        public void AddCommandAction(Type commandType)
        {
            if (typeof(ICommandAction).IsAssignableFrom(commandType) == false || commandType.IsInterface == true)
            {
                throw new InvalidCastException(nameof(commandType));
            }

            var cmdAttributeType = typeof(CommandAttribute);
            var multipleCmdAttributeType = typeof(MultipleCmdAttribute);
            var localCmdAttributeType = typeof(LocalCmdAttribute);

            var commandNames = new List<string>();
            bool isLocalCommand = false;
            if (commandType.IsDefined(cmdAttributeType) == true)
            {
                var attr = commandType.GetCustomAttribute(cmdAttributeType) as CommandAttribute;
                commandNames.Add(attr.CommandName);
            }
            else if (commandType.IsDefined(multipleCmdAttributeType) == true)
            {
                var attr = commandType.GetCustomAttribute(multipleCmdAttributeType) as MultipleCmdAttribute;
                foreach (var name in attr.Names)
                {
                    commandNames.Add(name);
                }
            }
            else if (commandType.IsDefined(localCmdAttributeType))
            {
                var attr = commandType.GetCustomAttribute(localCmdAttributeType) as LocalCmdAttribute;
                commandNames.Add(attr.Name);
                isLocalCommand = true;

            }
            else
            {
                commandNames.Add(commandType.Name);
            }

            foreach (var commandName in commandNames)
            {
                if (isLocalCommand)
                {
                    _commandTable.AddLoaclCommand(commandName);
                }
                else
                {
                    _commandTable.AddCommand(commandName);
                }
                _commandServiceContainer.RegisterType(commandName, commandType, LifeScope.Transient);
            }
        }

        public string GetModuleName()
        {
            return _moduleName;
        }
    }
}
