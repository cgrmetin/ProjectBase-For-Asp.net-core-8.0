using System.ComponentModel.DataAnnotations;

namespace ProjectBase.Entity.Request.Auth
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
