using ProjectBase.Core.Abstract;
using ProjectBase.Data.Concrete;
using ProjectBase.Data.Context;
using ProjectBase.Entity.Attributes;
using ProjectBase.Entity.Database;

namespace ProjectBase.Core.Concrete
{
    [IocContainerItem(typeof(IResourceRepository))]
    public class ResourceRepository : BaseRepository<Resource>, IResourceRepository
    {
        public ResourceRepository(AppDbContext context) : base(context)
        {
        }
    }
}
