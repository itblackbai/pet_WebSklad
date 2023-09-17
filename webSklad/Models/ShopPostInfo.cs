using System.ComponentModel.DataAnnotations;
using static System.Net.WebRequestMethods;

namespace webSklad.Models
{
    public class ShopPostInfo
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter the shop name.")]
        public string? Name { get; set; }
        public string? Type { get; set; }




        //Connections
        public List<Fop> FOPS { get; set; }
        public List<SalesRepresentative> SalesRepresentatives { get; set; }


        public string? UserId { get; set; } 
        public User User { get; set; } 

    }
}
