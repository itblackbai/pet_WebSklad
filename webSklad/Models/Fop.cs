using System.ComponentModel.DataAnnotations;

namespace webSklad.Models
{
    public class Fop
    {
        public int Id { get; set; }
        public string? NameOfFop { get; set; }

        public string? PhoneNumberOfFop { get; set; }

        public string? FopIPN { get; set; }

        public string? FopMFO { get; set; }

        public string? FopCODE { get; set; }

        public string? Account { get; set; }

        public string? Address { get; set; }


        //Connections
        public int? ShopPostInfoId { get; set; } 
        public ShopPostInfo ShopPostInfo { get; set; } 
    }
}
