namespace CommandSystem.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : System.Attribute
    {
        public string CommandName { get; private set; }

        public CommandAttribute(string name)
        {
            CommandName = name;
        }
    }
}
