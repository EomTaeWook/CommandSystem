using Dignus.Collections;

namespace CommandSystem.Models
{
    internal class CommandTable
    {
        private UniqueSet<string> _commands = new();
        private UniqueSet<string> _localCommands = new();
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
