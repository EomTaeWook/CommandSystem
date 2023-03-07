namespace CLISystem.Attribude
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MultipleCmdAttribute : Attribute
    {
        public List<string> Names { get; private set; }

        public MultipleCmdAttribute(params string[] names)
        {
            this.Names = names.ToList();
        }
    }
}
