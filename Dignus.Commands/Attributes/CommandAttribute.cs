namespace Dignus.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public string CommandName { get; private set; }

        public CommandAttribute(string name)
        {
            CommandName = name;
        }
    }
}
