using System.ComponentModel.DataAnnotations;

namespace webSklad.Models.ViewModels
{
    public class CreateShopPostInfoViewModel
    {
        [Required(ErrorMessage = "Please enter the shop name.")]
        public string? Name { get; set; }
        public string? Type { get; set; }
    }
}
