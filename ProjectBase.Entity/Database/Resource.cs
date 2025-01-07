using ProjectBase.Entity.Attributes;
using ProjectBase.Entity.Concrete;
using System.ComponentModel.DataAnnotations;

namespace ProjectBase.Entity.Database
{
    [DbObject]
    public class Resource : BaseEntity
    {
        [StringLength(100)]
        public string Code { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
    }
}
