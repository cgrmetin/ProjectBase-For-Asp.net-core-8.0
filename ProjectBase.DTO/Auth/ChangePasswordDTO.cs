using System.ComponentModel.DataAnnotations;

namespace ProjectBase.DTO.Auth
{
    public class ChangePasswordDTO
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*[.#$^+=!*()@%&]).{8,}$", ErrorMessage = "Şifreler En Az 8 Karakter ve 1 Büyük ,1 Küçük, 1 Rakam ve 1 Özel Karakter İçermeli")]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword")]
        public string PasswordConfirm { get; set; }
    }
}
