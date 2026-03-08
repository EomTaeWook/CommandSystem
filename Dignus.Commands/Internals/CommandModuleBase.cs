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
                    type.GetCustomAttributes<GlobalCommandAttribute>().Any())
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
        public void AddCommand(string path, string command, string desc, Func<string[], IActorRef, CancellationToken, Task> action)
        {
            AddCommandInternal(path, command, new ActionCommand(action, desc), false);
        }
        public void AddCommand(string command, string desc, Func<string[], IActorRef, CancellationToken, Task> action)
        {
            AddCommand(string.Empty, command, desc, action);
        }
        private void AddCommandInternal<T>(string commandPath, string commandName, T command, bool isGlobalCommand) where T : class, IPathCommand
        {
            if (isGlobalCommand == true)
            {
                _commandTable.AddGlobalCommand(commandName, typeof(T));
            }
            else
            {
                _commandTable.AddCommand(commandPath, commandName, typeof(T));
            }

            _serviceContainer.RegisterType(commandName, command);
        }

        public void AddCommand<T>(T command) where T : class, IPathCommand
        {
            var commandType = command.GetType();
            var commandAttrType = typeof(CommandAttribute);
            var globalCommandAttrType = typeof(GlobalCommandAttribute);

            if (commandType.IsDefined(commandAttrType))
            {
                var attr = commandType.GetCustomAttribute<CommandAttribute>();
                AddCommandInternal(attr.CommandPath, attr.CommandName, command, false);
            }
            else if (commandType.IsDefined(globalCommandAttrType))
            {
                var attrs = commandType.GetCustomAttributes<GlobalCommandAttribute>();
                foreach (var attr in attrs)
                {
                    AddCommandInternal(string.Empty, attr.CommandName, command, true);
                }
            }
        }
        public void AddCommand<T>() where T : IPathCommand
        {
            AddCommand(typeof(T));
        }

        public void AddCommand(Type commandType)
        {
            if (typeof(IPathCommand).IsAssignableFrom(commandType) == false || commandType.IsInterface == true)
            {
                throw new InvalidCastException(nameof(commandType));
            }

            var commandAttrType = typeof(CommandAttribute);
            var globalCommandAttrType = typeof(GlobalCommandAttribute);

            var commandNames = new List<string>();
            var commandPath = string.Empty;
            bool isGlobalCommand = false;
            if (commandType.IsDefined(commandAttrType) == true)
            {
                var attr = commandType.GetCustomAttribute<CommandAttribute>();
                commandPath = attr.CommandPath;
                commandNames.Add(attr.CommandName);
            }
            else if (commandType.IsDefined(globalCommandAttrType) == true)
            {
                isGlobalCommand = true;

                var attrs = commandType.GetCustomAttributes<GlobalCommandAttribute>();
                foreach (var attr in attrs)
                {
                    commandNames.Add(attr.CommandName);
                }
            }
            else
            {
                commandNames.Add(commandType.Name);
            }

            foreach (var commandName in commandNames)
            {
                if (isGlobalCommand)
                {
                    _commandTable.AddGlobalCommand(commandName, commandType);
                }
                else
                {
                    _commandTable.AddCommand(commandPath, commandName, commandType);
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
