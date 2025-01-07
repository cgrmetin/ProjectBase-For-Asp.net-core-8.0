using ProjectBase.Entity.Attributes;
using ProjectBase.Entity.Concrete;
using System.ComponentModel.DataAnnotations;

namespace ProjectBase.Entity.Database
{
    [DbObject]
    public class ResourceValue : BaseEntity
    {
        [StringLength(100)]
        public string ResourceCode { get; set; }
        [StringLength(10)]
        public string LanguageCode { get; set; }
        [StringLength(100)]
        public string Key { get; set; }
        [StringLength(500)]
        public string Value { get; set; }
    }
}
