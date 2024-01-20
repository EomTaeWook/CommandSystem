using Dignus.Collections;
using Dignus.DependencyInjection;

namespace CommandSystem.Internal
{
    internal class CommandProvider : ServiceProvider
    {
        private readonly Dictionary<string, Type> _typeNameToTypeMapping = new();
        private readonly Dictionary<string, object> _singletonObjects = new();
        private readonly UniqueSet<Type> _registeredTypeSet = new UniqueSet<Type>();

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
            base.AddTransient(implementationType, implementationType);
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
            base.AddSingleton(implementationType, implementationType);
        }
        public void AddSingleton<TService>(string typeName, TService implementation)
        {
            var type = typeof(TService);
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentException("service type name cannot be null or empty.", type.Name);
            }
            _typeNameToTypeMapping.Add(typeName, type);
            _singletonObjects.Add(typeName, implementation);
        }
        public T GetService<T>(string typeName)
        {
            if (_singletonObjects.TryGetValue(typeName, out object value))
            {
                return (T)value;
            }

            if (_typeNameToTypeMapping.TryGetValue(typeName, out Type type) == false)
            {
                throw new Exception("the registered resolve type could not be found. " + typeName);
            }

            return (T)this.GetService(type);
        }
    }
}
