using System.ComponentModel.DataAnnotations;

namespace webSklad.Models.ViewModels
{
    public class EDITSRViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Enter name")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯіІїЇєЄёЁ\s-]*$", ErrorMessage = "The name cannot contain numbers and symbols")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must contain between 2 and 50 characters")]
        [Display(Name = "Name")]
        public string? SRName { get; set; }
        [Required(ErrorMessage = "Enter last name")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯіІїЇєЄёЁ\s-]*$", ErrorMessage = "Last name cannot contain numbers and characters")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must contain between 2 and 50 characters")]
        [Display(Name = "Surname")]
        public string? SRSurname { get; set; }
        [Required(ErrorMessage = "Input Email")]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Email format is incorrect")]
        [EmailAddress(ErrorMessage = "Email must contain @ and .")]
        [Display(Name = "Email")]
        public string? SREmail { get; set; }
        [Required(ErrorMessage = "Enter a phone number")]
        [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Phone number must start with +380 and contain 13 characters +380XXXXXXXXX.")]
        [Display(Name = "Phone Number")]
        public string? SRPhoneNumber { get; set; }
    }
}
