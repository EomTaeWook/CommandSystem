using CLISystem.Interface;
using Kosher.Collections;
using Kosher.Framework;
using System.Collections;

namespace CLISystem.Models
{
    internal class ProcessorNames : IEnumerable<string>
    {
        private readonly Vector<string> _names = new Vector<string>();
        private readonly ServiceProvider _provider;
        public ProcessorNames(ServiceProvider serviceProvider)
        {
            _provider = serviceProvider;
        }
        public void Add(string name)
        {
            _names.Add(name);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _names.GetEnumerator();
        }
        public ICmdProcessor GetCmdProcessor(string name)
        {
            return _provider.GetService<ICmdProcessor>(name);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _names.GetEnumerator();
        }
    }
}
