namespace CommandSystem.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CmdAttribute : System.Attribute
    {
        public string Name { get; private set; }

        public CmdAttribute(string name)
        {
            Name = name;
        }
    }
}
