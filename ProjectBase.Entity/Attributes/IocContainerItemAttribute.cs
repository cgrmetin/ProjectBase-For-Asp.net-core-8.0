using Microsoft.Extensions.DependencyInjection;

namespace ProjectBase.Entity.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IocContainerItemAttribute : Attribute
    {
        public Type? ServiceType { get; }
        public ServiceLifetime Lifetime { get; }

        public IocContainerItemAttribute(Type? serviceType = null, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            ServiceType = serviceType;
            Lifetime = lifetime;
        }
    }
}
