using CommandSystem.Attributes;
using CommandSystem.Net.Handler;
using Dignus.Framework.Interfaces;

namespace CommandSystem.Middlewares
{
    internal class ActionAttributeMiddleware<THandler> : IMiddleware<(THandler, int, string)> where THandler : IProtocolHandlerContext
    {
        private readonly ActionAttribute _actionAttribute;
        public ActionAttributeMiddleware(ActionAttribute actionAttribute)
        {
            _actionAttribute = actionAttribute;
        }

        public async Task InvokeAsync((THandler, int, string) context, Func<Task> next)
        {
            if (_actionAttribute.ActionExecute(context.Item1) == false)
            {
                return;
            }
            await next();
        }
    }
}
