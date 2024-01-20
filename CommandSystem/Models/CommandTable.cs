using Dignus.Collections;

namespace CommandSystem.Models
{
    internal class CommandTable
    {
        private UniqueSet<string> _commands = new UniqueSet<string>();
        public void AddCommand(string commandName)
        {
            _commands.Add(commandName);
        }
        public IEnumerable<string> GetCommandList()
        {
            return _commands;
        }
    }
}
