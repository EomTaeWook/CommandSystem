using CommandSystem.Attributes;
using CommandSystem.Middlewares;
using CommandSystem.Net.Handler;
using Dignus.Log;
using Dignus.Pipeline;
using Dignus.Sockets;
using Dignus.Sockets.Attributes;
using Dignus.Sockets.Interfaces;
using System.Reflection;

namespace CommandSystem.Net
{
    public struct Context<THandler> where THandler : class, IProtocolHandler<string>
    {
        public THandler Handler { get; set; }
        public int Protocol { get; set; }

        public string Body { get; set; }
    }


    internal class HandlerFilterInvoker<THandler> where THandler : class, IProtocolHandler<string>, IProtocolHandlerContext
    {
        private static RefMiddlewarePipeline<Context<THandler>>[] _middlewarePipeline;
        public static void BindProtocol<TProtocol>() where TProtocol : struct, Enum
        {
            ProtocolHandlerMapper<THandler, string>.BindProtocol<TProtocol>();

            var handlerType = typeof(THandler);
            var protocolNames = Enum.GetNames(typeof(TProtocol));

            _middlewarePipeline = new RefMiddlewarePipeline<Context<THandler>>[protocolNames.Length];
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

                var middlewarePipeline = new RefMiddlewarePipeline<Context<THandler>>();

                var actionMiddleware = new ActionAttributeMiddleware<THandler>(orderedFilters);

                foreach (var filter in orderedFilters)
                {
                    middlewarePipeline.Use(actionMiddleware);
                }

                middlewarePipeline.Use((ref Context<THandler> context, RefMiddlewareNext<Context<THandler>> next) =>
                {
                    ProtocolHandlerMapper.DispatchToHandler(context.Handler, context.Protocol, context.Body);
                });
                _middlewarePipeline[index] = middlewarePipeline;
            }
        }

        public static void ExecuteProtocolHandler(THandler protocolHandler, int protocol, string body)
        {
            if (_middlewarePipeline[protocol] == null)
            {
                LogHelper.Fatal($"not found middleware. protocol number : {protocol}");
                return;
            }

            var context = new Context<THandler>()
            {
                Handler = protocolHandler,
                Body = body,
                Protocol = protocol,
            };

            _middlewarePipeline[protocol].Invoke(ref context);
        }
    }
}
