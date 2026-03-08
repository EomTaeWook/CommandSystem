namespace Dignus.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MultipleCommandAttribute : Attribute
    {
        public List<string> Names { get; private set; }

        public MultipleCommandAttribute(params string[] names)
        {
            Names = names.ToList();
        }
    }
}
