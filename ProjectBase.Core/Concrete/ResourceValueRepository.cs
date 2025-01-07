using ProjectBase.Core.Abstract;
using ProjectBase.Data.Concrete;
using ProjectBase.Data.Context;
using ProjectBase.Entity.Attributes;
using ProjectBase.Entity.Database;

namespace ProjectBase.Core.Concrete
{
    [IocContainerItem(typeof(IResourceValueRepository))]
    public class ResourceValueRepository : BaseRepository<ResourceValue>, IResourceValueRepository
    {
        public ResourceValueRepository(AppDbContext context) : base(context)
        {
        }
    }
}
