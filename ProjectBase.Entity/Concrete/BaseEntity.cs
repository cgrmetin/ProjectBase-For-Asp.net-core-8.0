using ProjectBase.Entity.Abstract;

namespace ProjectBase.Entity.Concrete
{
    public class BaseEntity : IBaseEntity
    {
        public int Id { get ; set ; }
        public DateTime? CreatedDate { get ; set ; }
        public DateTime? UpdatedDate { get ; set ; }
    }
}
