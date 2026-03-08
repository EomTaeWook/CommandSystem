using Dignus.Actor.Core.Actors;
using Dignus.Commands.Attributes;
using Dignus.Commands.Commands;
using Dignus.Commands.Interfaces;
using Dignus.DependencyInjection;
using Dignus.DependencyInjection.Extensions;
using System.Reflection;
using System.Text.Json;

namespace Dignus.Commands.Internals
{
    public abstract class CommandModuleBase
    {
        internal protected IServiceProvider _serviceProvider;

        private readonly ServiceContainer _serviceContainer;

        private readonly CommandTable _commandTable = new();
        private bool _isBuilt = false;
        private readonly string _moduleName;

        public CommandModuleBase(string moduleName)
        {
            if (string.IsNullOrEmpty(moduleName) == true)
            {
                moduleName = Assembly.GetEntryAssembly().GetName().Name;
            }
            _moduleName = moduleName;
            _serviceContainer = new ServiceContainer();
        }

        private void RegisterCommandActions(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes<CommandAttribute>().Any() ||
                    type.GetCustomAttributes<MultipleCommandAttribute>().Any() ||
                    type.GetCustomAttributes<LocalCommandAttribute>().Any())
                {
                    AddCommand(type);
                }
            }
        }
        internal void BuildInternal()
        {
            if (_isBuilt == true)
            {
                throw new InvalidOperationException("command module has already been built.");
            }
            _isBuilt = true;

            var assembly = Assembly.GetCallingAssembly();
            _serviceContainer.RegisterDependencies(assembly);
            RegisterCommandActions(assembly);

            _serviceContainer.RegisterType(_commandTable);
            _serviceContainer.RegisterType(_serviceContainer);

            if (File.Exists(AliasTable.Path) == true)
            {
                var alias = JsonSerializer.Deserialize<List<AliasModel>>(File.ReadAllText(AliasTable.Path));
                _serviceContainer.RegisterType(new AliasTable(alias));
            }
            else
            {
                _serviceContainer.RegisterType(new AliasTable([]));
            }
            _serviceProvider = _serviceContainer.Build();
        }

        public void AddCommand(string command, string desc, Func<string[], IActorRef, CancellationToken, Task> action)
        {
            AddCommand(command, new ActionCommand(action, desc), false);
        }
        private void AddCommand<T>(string commandName, T command, bool isLocalCommand) where T : class, ICommand
        {
            if (isLocalCommand == true)
            {
                _commandTable.AddLoaclCommand(commandName, typeof(T));
            }
            else
            {
                _commandTable.AddCommand(commandName, typeof(T));
            }

            _serviceContainer.RegisterType(commandName, command);
        }

        public void AddCommand<T>(T command) where T : class, ICommand
        {
            var commandType = command.GetType();
            var cmdAttributeType = typeof(CommandAttribute);
            var multipleCmdAttributeType = typeof(MultipleCommandAttribute);
            var localCmdAttributeType = typeof(LocalCommandAttribute);

            if (commandType.IsDefined(localCmdAttributeType))
            {
                var attr = commandType.GetCustomAttribute<LocalCommandAttribute>();
                AddCommand(attr.Name, command, true);
            }
            else if (commandType.IsDefined(multipleCmdAttributeType))
            {
                var attr = commandType.GetCustomAttribute<MultipleCommandAttribute>();
                foreach (var item in attr.Names)
                {
                    AddCommand(item, command, false);
                }
            }
            else if (commandType.IsDefined(cmdAttributeType))
            {
                var attr = commandType.GetCustomAttribute<CommandAttribute>();
                AddCommand(attr.CommandName, command, false);
            }
        }
        public void AddCommand<T>() where T : ICommand
        {
            AddCommand(typeof(T));
        }
        public void AddCommand(Type commandType)
        {
            if (typeof(ICommand).IsAssignableFrom(commandType) == false || commandType.IsInterface == true)
            {
                throw new InvalidCastException(nameof(commandType));
            }

            var cmdAttributeType = typeof(CommandAttribute);
            var multipleCmdAttributeType = typeof(MultipleCommandAttribute);
            var localCmdAttributeType = typeof(LocalCommandAttribute);

            var commandNames = new List<string>();
            bool isLocalCommand = false;
            if (commandType.IsDefined(cmdAttributeType) == true)
            {
                var attr = commandType.GetCustomAttribute(cmdAttributeType) as CommandAttribute;
                commandNames.Add(attr.CommandName);
            }
            else if (commandType.IsDefined(multipleCmdAttributeType) == true)
            {
                var attr = commandType.GetCustomAttribute(multipleCmdAttributeType) as MultipleCommandAttribute;
                foreach (var name in attr.Names)
                {
                    commandNames.Add(name);
                }
            }
            else if (commandType.IsDefined(localCmdAttributeType))
            {
                var attr = commandType.GetCustomAttribute(localCmdAttributeType) as LocalCommandAttribute;
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
                    _commandTable.AddLoaclCommand(commandName, commandType);
                }
                else
                {
                    _commandTable.AddCommand(commandName, commandType);
                }

                _serviceContainer.RegisterType(commandName, commandType, LifeScope.Transient);
            }
        }

        public string GetModuleName()
        {
            return _moduleName;
        }
    }
}
