using Dignus.Collections;

namespace CommandSystem.Internals
{
    internal class CommandTable
    {
        private readonly UniqueSet<string> _commands = [];
        private readonly UniqueSet<string> _localCommands = [];
        public void AddCommand(string commandName)
        {
            _commands.Add(commandName);
        }
        public void AddLoaclCommand(string commandName)
        {
            _localCommands.Add(commandName);
        }
        public bool IsContainLocalCommand(string commandName)
        {
            return _localCommands.Contains(commandName);
        }
        public IEnumerable<string> GetCommandList()
        {
            return _commands;
        }
        public IEnumerable<string> GetLocalCommandList()
        {
            return _localCommands;
        }
    }
}
