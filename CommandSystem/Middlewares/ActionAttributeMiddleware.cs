using CommandSystem.Attributes;
using CommandSystem.Net;
using CommandSystem.Net.Handler;
using Dignus.Pipeline;
using Dignus.Sockets.Interfaces;

namespace CommandSystem.Middlewares
{
    internal class ActionAttributeMiddleware<THandler> : IRefMiddleware<Context<THandler>> where THandler : class, IProtocolHandlerContext, IProtocolHandler<string>
    {
        private readonly List<ActionAttribute> _actionAttributes;
        public ActionAttributeMiddleware(List<ActionAttribute> actionAttributes)
        {
            _actionAttributes = actionAttributes;
        }

        public void Invoke(ref Context<THandler> context, RefMiddlewareNext<Context<THandler>> next)
        {
            foreach (var actionAttribute in _actionAttributes)
            {
                if (actionAttribute.ActionExecute(context.Handler) == false)
                {
                    return;
                }
                next(ref context);
            }
        }
    }
}
