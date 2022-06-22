using System.ComponentModel.DataAnnotations;

namespace Service.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Логин (Email)")]
        public string? Email { get; set; }

        [Required]
        [UIHint("password")]
        [Display(Name = "Пароль")]
        public string? Password { get; set; }
    }
}
