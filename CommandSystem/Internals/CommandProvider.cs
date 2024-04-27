namespace CommandSystem.Internals
{
    internal class CommandProvider
    {
        private readonly Dictionary<string, object> _typeNameToSingletonMap = new();
        private readonly Dictionary<string, Type> _typeNameToTypeMapping = new();

        private readonly IServiceProvider _serviceProvider;
        public CommandProvider(IServiceProvider serviceProvider, CommandServiceCollection serviceRegistrations)
        {
            _serviceProvider = serviceProvider;
            _typeNameToTypeMapping = serviceRegistrations._typeNameToTypeMapping;
            _typeNameToSingletonMap = serviceRegistrations._typeNameToSingletonMap;
        }

        public T GetService<T>(string typeName)
        {
            if (_typeNameToSingletonMap.TryGetValue(typeName, out object value))
            {
                return (T)value;
            }

            if (_typeNameToTypeMapping.TryGetValue(typeName, out Type type) == false)
            {
                throw new Exception("the registered resolve type could not be found. " + typeName);
            }

            return (T)_serviceProvider.GetService(type);
        }
        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }
        public object GetService(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }
}
