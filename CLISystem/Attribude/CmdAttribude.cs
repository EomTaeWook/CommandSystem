namespace CLISystem.Attribude
{
    public class CmdAttribude : Attribute
    {
        public string Name { get; private set; }

        public CmdAttribude(string Name)
        {
            this.Name = Name;
        }
    }
}
