namespace CommandSystem.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MultipleCmdAttribute : System.Attribute
    {
        public List<string> Names { get; private set; }

        public MultipleCmdAttribute(params string[] names)
        {
            Names = names.ToList();
        }
    }
}
