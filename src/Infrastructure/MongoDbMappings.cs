using System.Reflection;

namespace Infrastructure;

public static partial class DependencyInjection
{
    internal static class MongoDbMappings
    {
        public static void RegisterMappings(Assembly assembly)
        {
            IEnumerable<Type> mappingTypes = assembly.GetTypes()
                                .Where(t => t.IsClass && !t.IsAbstract && typeof(IMappingConfigurationEntity).IsAssignableFrom(t));

            foreach (Type mappingType in mappingTypes)
            {
                object? mappingInstance = Activator.CreateInstance(mappingType);
                MethodInfo? configureMethod = mappingType.GetMethod("Configure");
                configureMethod!.Invoke(mappingInstance, null);
            }
        }
    }
}

