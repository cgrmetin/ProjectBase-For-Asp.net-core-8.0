using System.ComponentModel.DataAnnotations;

namespace ProjectBase.Entity.Abstract
{
    public interface IBaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
