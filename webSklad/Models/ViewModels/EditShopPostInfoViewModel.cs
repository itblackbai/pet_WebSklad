using System.ComponentModel.DataAnnotations;

namespace webSklad.Models.ViewModels
{
    public class EditShopPostInfoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the shop name.")]
        public string? Name { get; set; }

        
    }
}
