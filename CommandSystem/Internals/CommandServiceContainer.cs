using Dignus.DependencyInjection;
using Dignus.DependencyInjection.Extensions;
using Dignus.DependencyInjection.Interfaces;

namespace CommandSystem.Internals
{
    internal class CommandServiceContainer : IServiceContainer
    {
        private CommandProvider _serviceProvider;
        private readonly CommandServiceCollection _commandServiceCollection = new();

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

        public void RegisterType(Type serviceType, Type implementationType, LifeScope lifeScope)
        {
            if (lifeScope == LifeScope.Transient)
            {
                _commandServiceCollection.AddTransient(serviceType, implementationType);
            }
            else
            {
                _commandServiceCollection.AddSingleton(serviceType, implementationType);
            }
        }

        object IServiceContainer.Resolve(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }
}
