using CommandSystem.Attributes;
using CommandSystem.Net.Middlewares;
using Dignus.Framework.Pipeline;
using Dignus.Framework.Pipeline.Interfaces;

namespace CommandSystem.Middlewares
{
    internal class ActionAttributeMiddleware : IAsyncMiddleware<CSPipeContext>
    {
        private readonly List<ActionAttribute> _actionAttributes;
        public ActionAttributeMiddleware(List<ActionAttribute> actionAttributes)
        {
            _actionAttributes = actionAttributes;
        }

        public Task InvokeAsync(ref CSPipeContext context, ref AsyncPipelineNext<CSPipeContext> next)
        {
            foreach (var actionAttribute in _actionAttributes)
            {
                if (actionAttribute.ActionExecute(context) == false)
                {
                    return Task.CompletedTask;
                }
            }
            return next.InvokeAsync(ref context);
        }
    }
}
