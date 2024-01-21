using Dignus.DependencyInjection;
using Dignus.DependencyInjection.Extensions;

namespace CommandSystem.Internal
{
    internal class CommandServiceContainer
    {
        private CommandProvider _serviceProvider;

        private CommandServiceCollection _commandServiceCollection = new();
        public CommandServiceContainer()
        {
        }
        public IServiceProvider Build()
        {
            var serviceProvider = _commandServiceCollection.BuildServiceProvider();
            _serviceProvider = new CommandProvider(serviceProvider, _commandServiceCollection);

            return serviceProvider;
        }
        public void RegisterType(string typeName, Type implementationType, LifeScope lifeScope)
        {
            if (lifeScope == LifeScope.Transient)
            {
                _commandServiceCollection.AddTransient(typeName, implementationType);
            }
            else
            {
                _commandServiceCollection.AddSingleton(typeName, implementationType);
            }
        }
        public void RegisterType<TService>(string typeName, TService implementation) where TService : class
        {
            _commandServiceCollection.AddSingleton(typeName, implementation);
        }
        public void RegisterType<TService>(TService implementation) where TService : class
        {
            _commandServiceCollection.AddSingleton(implementation);
        }
        public T Resolve<T>()
        {
            return _serviceProvider.GetService<T>();
        }
        public T Resolve<T>(string typeName)
        {
            return _serviceProvider.GetService<T>(typeName);
        }
    }
}
