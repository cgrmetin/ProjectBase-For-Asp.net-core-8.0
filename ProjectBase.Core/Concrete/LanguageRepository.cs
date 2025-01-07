using ProjectBase.Core.Abstract;
using ProjectBase.Data.Concrete;
using ProjectBase.Data.Context;
using ProjectBase.Entity.Attributes;
using ProjectBase.Entity.Database;

namespace ProjectBase.Core.Concrete
{
    [IocContainerItem(typeof(ILanguageRepository))]
    public class LanguageRepository : BaseRepository<Language>, Abstract.ILanguageRepository
    {
        public LanguageRepository(AppDbContext context) : base(context)
        {
        }
    }
}
