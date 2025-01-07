using System.ComponentModel.DataAnnotations;
using static ProjectBase.Entity.Enum.GlobalEnum;

namespace ProjectBase.DTO.Auth
{
    public class RegistrationDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*[.#$^+=!*()@%&]).{8,}$", ErrorMessage = "Şifreler En Az 8 Karakter ve 1 Büyük ,1 Küçük, 1 Rakam ve 1 Özel Karakter İçermeli")]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string PasswordRepeat { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string? Role { get; set; }
        public UserType UserType { get; set; }
        [Required]
        public bool AcceptTerms { get; set; }
    }
}
