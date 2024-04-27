using CommandSystem.Attributes;
using CommandSystem.Middlewares;
using CommandSystem.Net.Handler;
using Dignus.Framework;
using Dignus.Log;
using Dignus.Sockets;
using Dignus.Sockets.Attributes;
using Dignus.Sockets.Interfaces;
using System.Reflection;

namespace CommandSystem.Net
{
    internal class HandlerFilterInvoker<THandler> where THandler : class, IProtocolHandler<string>, IProtocolHandlerContext
    {
        private static Pipeline<(THandler, int, string)>[] _middlewarePipeline;
        public static void BindProtocol<TProtocol>() where TProtocol : struct, Enum
        {
            ProtocolHandlerMapper<THandler, string>.BindProtocol<TProtocol>();

            var handlerType = typeof(THandler);
            var protocolNames = Enum.GetNames(typeof(TProtocol));

            _middlewarePipeline = new Pipeline<(THandler, int, string)>[protocolNames.Length];
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

                var middlewarePipeline = new Pipeline<(THandler, int, string)>();
                foreach (var filter in orderedFilters)
                {
                    middlewarePipeline.Use(new ActionAttributeMiddleware<THandler>(filter));
                }
                middlewarePipeline.Use((conext, next) =>
                {
                    ProtocolHandlerMapper.DispatchToHandler(conext.Item1, conext.Item2, conext.Item3);
                    return next();
                });
                _middlewarePipeline[index] = middlewarePipeline;
            }
        }

        public static Task ExecuteProtocolHandler(THandler protocolHandler, int protocol, string body)
        {
            if (_middlewarePipeline[protocol] == null)
            {
                LogHelper.Fatal($"not found middleware. protocol number : {protocol}");
                return Task.CompletedTask;
            }
            var context = new ValueTuple<THandler, int, string>(protocolHandler, protocol, body);

            return _middlewarePipeline[protocol].InvokeAsync(context);
        }
    }
}
