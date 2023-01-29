namespace CLISystem.Attribude
{
    public class CmdAttribute : Attribute
    {
        public string Name { get; private set; }

        public CmdAttribute(string Name)
        {
            this.Name = Name;
        }
    }
}
