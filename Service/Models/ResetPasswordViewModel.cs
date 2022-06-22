using System.ComponentModel.DataAnnotations;

namespace Service.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage="Введите пароль")]
        [Display(Name = "Новый пароль")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают!")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Введите email")]
        [Display(Name = "Введите email")]
        public string? Email { get; set; }
        public string? Code { get; set; }
    }
}
