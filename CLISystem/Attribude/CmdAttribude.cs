namespace CLISystem.Attribude
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CmdAttribute : Attribute
    {
        public string Name { get; private set; }

        public CmdAttribute(string name)
        {
            this.Name = name;
        }
    }
}
