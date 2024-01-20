using Dignus.DependencyInjection;

namespace CommandSystem.Internal
{
    internal class CommandServiceContainer
    {
        internal CommandProvider _serviceProvider = new();

        public CommandServiceContainer()
        {
        }
        public void RegisterType(string typeName, Type implementationType, LifeScope lifeScope)
        {
            if (lifeScope == LifeScope.Transient)
            {
                _serviceProvider.AddTransient(typeName, implementationType);
            }
            else
            {
                _serviceProvider.AddSingleton(typeName, implementationType);
            }
        }
        public void RegisterType<TService>(string typeName, TService implementation) where TService : class
        {
            _serviceProvider.AddSingleton(typeName, implementation);
        }
        public void RegisterType<TService>(TService implementation) where TService : class
        {
            _serviceProvider.AddSingleton(implementation);
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
