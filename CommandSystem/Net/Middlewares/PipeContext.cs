using CommandSystem.Net.Handler;
using Dignus.Framework.Pipeline.Interfaces;
using Dignus.Sockets.Interfaces;

namespace CommandSystem.Net.Middlewares
{
    internal struct CSPipeContext : IMiddlewareContext<CSProtocolHandler, string>
    {
        public ISession Session { get; init; }
        public int Protocol { get; set; }

        public CSProtocolHandler Handler { get; set; }

        public string Body { get; set; }
    }

    internal struct SCPipeContext : IMiddlewareContext<SCProtocolHandler, string>
    {
        public int Protocol { get; set; }
        public SCProtocolHandler Handler { get; set; }
        public string Body { get; set; }
    }
}
