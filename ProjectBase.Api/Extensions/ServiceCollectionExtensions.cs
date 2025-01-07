using ProjectBase.Entity.Attributes;
using System.Reflection;

namespace ProjectBase.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServicesFromAttributes(this IServiceCollection services, params Assembly[] assemblies)
        {
            var typesWithAttribute = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttributes(typeof(IocContainerItemAttribute), false).Any())
                .ToList();

            foreach (var type in typesWithAttribute)
            {
                var attribute = (IocContainerItemAttribute)type.GetCustomAttributes(typeof(IocContainerItemAttribute), false).First();

                Console.WriteLine($"Type: {type.FullName}, Attribute: {attribute}");

                if (attribute.ServiceType == null)
                {
                    // ServiceType is null, register the type itself
                    switch (attribute.Lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            services.AddSingleton(type);
                            break;
                        case ServiceLifetime.Scoped:
                            services.AddScoped(type);
                            break;
                        case ServiceLifetime.Transient:
                            services.AddTransient(type);
                            break;
                        default:
                            throw new InvalidOperationException($"Unsupported lifetime for {type.FullName}");
                    }
                }
                else
                {
                    // Register with a specified service type
                    switch (attribute.Lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            services.AddSingleton(attribute.ServiceType, type);
                            break;
                        case ServiceLifetime.Scoped:
                            services.AddScoped(attribute.ServiceType, type);
                            break;
                        case ServiceLifetime.Transient:
                            services.AddTransient(attribute.ServiceType, type);
                            break;
                        default:
                            throw new InvalidOperationException($"Unsupported lifetime for {type.FullName}");
                    }
                }
            }

            return services;
        }
    }
}
