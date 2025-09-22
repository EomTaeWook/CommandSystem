using CommandSystem.Attributes;
using CommandSystem.Middlewares;
using Dignus.Log;
using Dignus.Pipeline;
using Dignus.Sockets;
using Dignus.Sockets.Attributes;
using Dignus.Sockets.Interfaces;
using System.Reflection;

namespace CommandSystem.Net
{
    public readonly struct PipeContext
    {
        public ISession Session { get; init; }
        public readonly IProtocolHandler<string> Handler { get; init; }
        public readonly int Protocol { get; init; }
        public readonly string Body { get; init; }
    }

    internal class HandlerFilterInvoker<THandler> where THandler : class, IProtocolHandler<string>
    {
        private static RefMiddlewarePipeline<PipeContext>[] _middlewarePipeline;
        public static void BindProtocol<TProtocol>() where TProtocol : struct, Enum
        {
            ProtocolHandlerMapper<THandler, string>.BindProtocol<TProtocol>();

            var handlerType = typeof(THandler);
            var protocolNames = Enum.GetNames(typeof(TProtocol));

            _middlewarePipeline = new RefMiddlewarePipeline<PipeContext>[protocolNames.Length];
            foreach (var method in handlerType.GetMethods())
            {
                var methodName = method.Name;

                var protocolNameAttr = method.GetCustomAttribute<ProtocolNameAttribute>();
                if (protocolNameAttr != null)
                {
                    methodName = protocolNameAttr.Name;
                }
                var index = Array.IndexOf(protocolNames, methodName);
                if (index == -1)
                {
                    continue;
                }
                var filters = method.GetCustomAttributes<ActionAttribute>();
                var orderedFilters = filters.OrderBy(r => r.Order).ToList();

                var middlewarePipeline = new RefMiddlewarePipeline<PipeContext>();

                var actionMiddleware = new ActionAttributeMiddleware(orderedFilters);

                foreach (var filter in orderedFilters)
                {
                    middlewarePipeline.Use(actionMiddleware);
                }

                middlewarePipeline.Use((ref PipeContext context, RefMiddlewareNext<PipeContext> next) =>
                {
                    ProtocolHandlerMapper.DispatchToHandler(context.Handler as THandler, context.Protocol, context.Body);
                });
                _middlewarePipeline[index] = middlewarePipeline;
            }
        }

        public static void ExecuteProtocolHandler(ISession session, THandler protocolHandler, int protocol, string body)
        {
            if (_middlewarePipeline[protocol] == null)
            {
                LogHelper.Fatal($"not found middleware. protocol number : {protocol}");
                return;
            }

            var context = new PipeContext()
            {
                Session = session,
                Handler = protocolHandler,
                Body = body,
                Protocol = protocol,
            };

            _middlewarePipeline[protocol].Invoke(ref context);
        }
    }
}
