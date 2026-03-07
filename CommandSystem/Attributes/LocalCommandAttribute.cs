namespace CommandSystem.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LocalCommandAttribute : Attribute
    {
        public string Name { get; private set; }

        public LocalCommandAttribute(string name)
        {
            Name = name;
        }
    }
}
