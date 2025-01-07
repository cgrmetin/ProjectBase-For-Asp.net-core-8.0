using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static ProjectBase.Entity.Enum.GlobalEnum;

namespace ProjectBase.Entity.Database
{
    public class AppUser : IdentityUser<int>
    {
        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }
        public UserType UserType { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
