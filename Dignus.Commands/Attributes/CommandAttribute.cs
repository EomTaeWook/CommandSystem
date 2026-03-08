namespace Dignus.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CommandAttribute(string path, string name) : Attribute
    {
        public string CommandName { get; private set; } = name;

        public string CommandPath { get; private set; } = path;
        public CommandAttribute(string name) : this(string.Empty, name)
        {
        }
    }
}
