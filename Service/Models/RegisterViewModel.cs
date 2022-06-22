using System.ComponentModel.DataAnnotations;

namespace Service.Models
{
    public class RegisterViewModel
    {
        [Required (ErrorMessage = "Введите имя")]
        [Display(Name = "Введите имя")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Введите фамилию")]
        [Display(Name = "Введите фамилию")]
        public string? LastName { get; set; }

        [Display(Name = "Введите отчество (если есть)")]
        public string? MiddleName { get; set; }

        [Required (ErrorMessage = "Введите группу")]
        [Display(Name = "Введите группу в формате МЕН-ХХХХХХ")]
        public string? Group { get; set; }

        [Required (ErrorMessage = "Введите Email")]
        [EmailAddress (ErrorMessage = "Некорректный адрес")]
        [Display(Name = "Введите Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Пароль должен содержать от 5 до 15 символов, иметь заглавную и маленькую буквы и цифры")]
        [MinLength (5)]
        [MaxLength (15)]
        [DataType(DataType.Password)]
        [Display(Name = "Введите пароль (должен содержать от 5 до 15 символов, иметь заглавную и маленькую буквы и цифры)")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        public string? PasswordConfirm { get; set; }
    }
}
