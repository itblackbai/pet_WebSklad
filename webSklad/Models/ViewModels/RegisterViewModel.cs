using System.ComponentModel.DataAnnotations;

namespace webSklad.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Внесіть ім'я")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯіІїЇєЄёЁ\s-]*$", ErrorMessage = "Ім'я не може містити цифр та символів")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Ім'я повинно містити від 2 до 50 символів")]
        [Display(Name = "Ім'я")]
        public string? NameUser { get; set; }

        [Required(ErrorMessage = "Внесіть прізвище")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯіІїЇєЄёЁ\s-]*$", ErrorMessage = "Прізвище не може містити цифр та символів")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Прізвище повинно містити від 2 до 50 символів")]
        [Display(Name = "Прізвище")]
        public string? SurnameUser { get; set; }

        [Required(ErrorMessage = "Внесіть номер телефону")]
        [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Номер телефону має починатися з +380 та містити 13 символів +380XXXXXXXXX.")]
        [Display(Name = "Номер телефону")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Внесіть Email")]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Некоректний формат Email")]
        [EmailAddress(ErrorMessage = "Email має містити @ та .")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Внесіть пароль")]
        [MinLength(3, ErrorMessage = "Пароль має містити від 3 символів")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Внесіть пароль повторно")]
        [DataType(DataType.Password)]
        [Display(Name = "Підтвердіть пароль")]
        [Compare("Password", ErrorMessage = "Паролі мають збігатися")]
        public string? ConfirmPassword { get; set; }

    }
}
