using CommandSystem.Net.Middlewares;

namespace CommandSystem.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal abstract class ActionAttribute(int order = 0) : System.Attribute
    {
        public int Order { get; private set; } = order;

        public abstract bool ActionExecute(CSPipeContext context);
    }
}
