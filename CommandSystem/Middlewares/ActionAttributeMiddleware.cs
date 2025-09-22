using CommandSystem.Attributes;
using CommandSystem.Net;
using Dignus.Pipeline;

namespace CommandSystem.Middlewares
{
    internal class ActionAttributeMiddleware : IRefMiddleware<PipeContext>
    {
        private readonly List<ActionAttribute> _actionAttributes;
        public ActionAttributeMiddleware(List<ActionAttribute> actionAttributes)
        {
            _actionAttributes = actionAttributes;
        }

        public void Invoke(ref PipeContext context, RefMiddlewareNext<PipeContext> next)
        {
            foreach (var actionAttribute in _actionAttributes)
            {
                if (actionAttribute.ActionExecute(context) == false)
                {
                    return;
                }
                next(ref context);
            }
        }
    }
}
