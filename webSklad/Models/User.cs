using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace webSklad.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "Enter name")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯіІїЇєЄёЁ\s-]*$", ErrorMessage = "The name cannot contain numbers and symbols")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must contain between 2 and 50 characters")]
        [Display(Name = "Name")]
        public string? NameUser { get; set; }
        [Required(ErrorMessage = "Enter last name")]
        [RegularExpression(@"^[a-zA-Zа-яА-ЯіІїЇєЄёЁ\s-]*$", ErrorMessage = "Last name cannot contain numbers and characters")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must contain between 2 and 50 characters")]
        [Display(Name = "Surname")]
        public string? SurnameUser { get; set; }
        public string? CreatedByUserId { get; set; }


        //Connections
        public List<ShopPostInfo> ShopPostInfos { get; set; }

    }
}
