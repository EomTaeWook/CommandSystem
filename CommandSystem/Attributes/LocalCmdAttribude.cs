namespace CommandSystem.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LocalCmdAttribute : System.Attribute
    {
        public string Name { get; private set; }

        public LocalCmdAttribute(string name)
        {
            Name = name;
        }
    }
}
