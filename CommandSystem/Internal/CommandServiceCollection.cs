using Dignus.Collections;
using Dignus.DependencyInjection;

namespace CommandSystem.Internal
{
    internal class CommandServiceCollection : ServiceCollection
    {
        internal readonly Dictionary<string, object> _typeNameToSingletonMap = new();
        internal readonly Dictionary<string, Type> _typeNameToTypeMapping = new();
        private readonly UniqueSet<Type> _registeredTypeSet = new();
        public void AddTransient(string typeName, Type implementationType)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentException("service type name cannot be null or empty.", implementationType.Name);
            }
            _typeNameToTypeMapping.Add(typeName, implementationType);
            if (_registeredTypeSet.Contains(implementationType))
            {
                return;
            }
            _registeredTypeSet.Add(implementationType);

            base.Add(new ServiceRegistration(implementationType, implementationType, LifeScope.Transient));
        }
        public void AddSingleton(string typeName, Type implementationType)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentException("service type name cannot be null or empty.", implementationType.Name);
            }
            _typeNameToTypeMapping.Add(typeName, implementationType);
            if (_registeredTypeSet.Contains(implementationType))
            {
                return;
            }
            _registeredTypeSet.Add(implementationType);
            base.Add(new ServiceRegistration(implementationType, implementationType, LifeScope.Singleton));
        }
        public void AddSingleton<TService>(string typeName, TService instance)
        {
            var type = typeof(TService);
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentException("service type name cannot be null or empty.", type.Name);
            }
            _typeNameToTypeMapping.Add(typeName, type);
            _typeNameToSingletonMap.Add(typeName, instance);
        }
    }
}
