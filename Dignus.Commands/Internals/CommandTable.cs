using Dignus.Collections;

namespace Dignus.Commands.Internals
{
    internal class CommandTable
    {
        private readonly UniqueSet<string> _commandNames = [];
        private readonly UniqueSet<string> _globalCommandNames = [];

        private readonly Dictionary<string, Type> _commandNameToTypeMapping = [];
        public void AddCommand(string path, string commandName, Type commandType)
        {
            var key = commandName;
            if(string.IsNullOrWhiteSpace(path) == false)
            {
                key = $"{path}/{commandName}";
            }
            _commandNames.Add(key);

            _commandNameToTypeMapping.Add(key, commandType);
        }

        public void AddGlobalCommand(string commandName, Type commandType)
        {
            _globalCommandNames.Add(commandName);
            _commandNameToTypeMapping.Add(commandName, commandType);
        }
        public ArrayQueue<string> GetCommandListByPath(string currentPath) 
        {
            var findNames = new ArrayQueue<string>();

            foreach (var command in _commandNames)
            {
                if(command.StartsWith(currentPath) == false)
                {
                    continue;
                }

                findNames.Add(command);
            }
            return findNames;
        }
        public Type GetCommandType(string commandKey)
        {
            if (_commandNameToTypeMapping.TryGetValue(commandKey, out var type))
            {
                return type;
            }

            return null;
        }
        public Type GetCommandType(string currentPath, string commandName)
        {
            string key;
            if(string.IsNullOrWhiteSpace(currentPath))
            {
                key = commandName;
            }
            else
            {
                key = $"{currentPath}/{commandName}";
            }

            return GetCommandType(key);
        }

        public Type GetGlobalCommandType(string commandName)
        {
            if(_globalCommandNames.Contains(commandName) == false)
            {
                return null;
            }

            if (_commandNameToTypeMapping.TryGetValue(commandName, out var type))
            {
                return type;
            }
            return null;
        }
        public IEnumerable<string> GetGlobalCommandList()
        {
            return _globalCommandNames;
        }
    }
}
