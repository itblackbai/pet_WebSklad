using System.ComponentModel.DataAnnotations;

namespace webSklad.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Enter name")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯіІїЇєЄёЁ\s-]*$", ErrorMessage = "The name cannot contain numbers and characters")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must contain between 2 and 50 characters")]
        [Display(Name = "Name")]
        public string? NameUser { get; set; }
        [Required(ErrorMessage = "Enter last name")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯіІїЇєЄёЁ\s-]*$", ErrorMessage = "Last name cannot contain numbers and characters")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must contain between 2 and 50 characters")]
        [Display(Name = "Surname")]
        public string? SurnameUser { get; set; }

        [Required(ErrorMessage = "Enter a phone number")]
        [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Phone number must start with +380 and contain 13 characters +380XXXXXXXXX.")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Enter Email")]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Incorrect Email Format")]
        [EmailAddress(ErrorMessage = "Email must contain @ and .")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Enter password")]
        [MinLength(3, ErrorMessage = "Password must contain at least 3 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Re-enter password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords must match")]
        public string? ConfirmPassword { get; set; }

    }
}
