using System.ComponentModel.DataAnnotations;

namespace webSklad.Models.ViewModels
{
    public class EditFopViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Enter name and surname")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯіІїЇєЄёЁ\s-]*$", ErrorMessage = "The name and surname cannot contain numbers and symbols")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name and surname must contain between 2 and 50 characters")]
        [Display(Name = "Full name ")]
        public string? FOPName { get; set; }
        [Required(ErrorMessage = "Enter a phone number")]
        [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Phone number must start with +380 and contain 13 characters +380XXXXXXXXX.")]
        [Display(Name = "Phone Number")]
        public string? FOPPhoneNumber { get; set; }

        [Required(ErrorMessage = "Enter IPN number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "IPN must contain exactly 10 digits")]
        [Display(Name = "IPN")]
        public string? FopIPN { get; set; }
        [Required(ErrorMessage = "Enter MFO number")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "MFO must contain exactly 6 digits")]
        [Display(Name = "MFO")]
        public string? FopMFO { get; set; }
        [Required(ErrorMessage = "Enter CODE number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "CODE must contain exactly 10 digits")]
        [Display(Name = "Code")]
        public string? FopCODE { get; set; }
        [Required(ErrorMessage = "Enter account number")]
        [RegularExpression(@"^UA-\d{2}\d{2}\d{6}\d{5,19}$", ErrorMessage = "Invalid account format")]
        [Display(Name = "Account")]
        public string? FOPAccount { get; set; }
        [Required(ErrorMessage = "Enter adress")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Adress must contain between 2 and 200 characters")]
        [Display(Name = "Adress")]
        public string? Address { get; set; }
    }
}
