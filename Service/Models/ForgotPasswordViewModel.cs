using System.ComponentModel.DataAnnotations;

namespace Service.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage="Введите email")]
        [Display(Name="Введите email")]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
