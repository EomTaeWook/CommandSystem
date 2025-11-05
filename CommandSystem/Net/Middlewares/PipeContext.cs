using CommandSystem.Net.Handler;
using Dignus.Sockets.Interfaces;
using Dignus.Sockets.Pipeline;

namespace CommandSystem.Net.Middlewares
{
    internal struct CSPipeContext : IPipelineContext<CSProtocolHandler, string>
    {
        public ISession Session { get; init; }
        public int Protocol { get; set; }

        public CSProtocolHandler Handler { get; set; }

        public string Body { get; set; }
    }

    internal struct SCPipeContext : IPipelineContext<SCProtocolHandler, string>
    {
        public int Protocol { get; set; }
        public SCProtocolHandler Handler { get; set; }
        public string Body { get; set; }
    }
}
