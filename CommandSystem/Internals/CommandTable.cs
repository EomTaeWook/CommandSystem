using Dignus.Collections;

namespace CommandSystem.Internals
{
    internal class CommandTable
    {
        private readonly UniqueSet<string> _commandNames = [];
        private readonly UniqueSet<string> _localCommandNames = [];

        private readonly Dictionary<string, Type> _commandNameToTypeMapping = [];
        public void AddCommand(string commandName, Type commandType)
        {
            _commandNames.Add(commandName);

            _commandNameToTypeMapping.Add(commandName, commandType);
        }

        public void AddLoaclCommand(string commandName, Type commandType)
        {
            _localCommandNames.Add(commandName);

            _commandNameToTypeMapping.Add(commandName, commandType);
        }

        public Type GetCommandType(string commandName)
        {
            if (_commandNameToTypeMapping.TryGetValue(commandName, out var type))
            {
                return type;
            }
            return null;
        }

        public bool IsContainLocalCommand(string commandName)
        {
            return _localCommandNames.Contains(commandName);
        }
        public IEnumerable<string> GetCommandList()
        {
            return _commandNames;
        }
        public IEnumerable<string> GetLocalCommandList()
        {
            return _localCommandNames;
        }
    }
}
