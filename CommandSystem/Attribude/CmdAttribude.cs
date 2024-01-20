namespace CommandSystem.Attribude
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CmdAttribute : Attribute
    {
        public string Name { get; private set; }

        public CmdAttribute(string name)
        {
            Name = name;
        }
    }
}
