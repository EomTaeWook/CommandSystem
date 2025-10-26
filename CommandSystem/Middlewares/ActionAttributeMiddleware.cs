using CommandSystem.Attributes;
using CommandSystem.Net.Middlewares;
using Dignus.Framework.Pipeline;

namespace CommandSystem.Middlewares
{
    internal class ActionAttributeMiddleware : IRefMiddleware<CSPipeContext>
    {
        private readonly List<ActionAttribute> _actionAttributes;
        public ActionAttributeMiddleware(List<ActionAttribute> actionAttributes)
        {
            _actionAttributes = actionAttributes;
        }

        public void Invoke(ref CSPipeContext context, RefMiddlewareNext<CSPipeContext> next)
        {
            foreach (var actionAttribute in _actionAttributes)
            {
                if (actionAttribute.ActionExecute(context) == false)
                {
                    return;
                }
            }
            next(ref context);
        }
    }
}
