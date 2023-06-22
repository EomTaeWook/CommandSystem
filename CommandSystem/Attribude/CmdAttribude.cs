using Dignus.DependencyInjection.Attribute;

namespace CommandSystem.Attribude
{
    [Injectable(Dignus.DependencyInjection.LifeScope.Transient)]
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
