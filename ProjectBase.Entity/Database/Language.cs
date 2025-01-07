using ProjectBase.Entity.Attributes;
using ProjectBase.Entity.Concrete;
using System.ComponentModel.DataAnnotations;

namespace ProjectBase.Entity.Database
{
    [DbObject]
    public class Language : BaseEntity
    {
        [StringLength(20)]
        public string Name{ get; set; }
        [StringLength(20)]
        public string OriginalName { get; set; }
        [StringLength(10)]
        public string Code { get; set; }

    }
}
