using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace webSklad.Models
{
    public class CartRegister
    {
        public int Id { get; set; }
        public DateTime? CartRegisterDateTime { get; set; }

        //public DateTime? CartRegisterDateTimenOW { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SoldAmount { get; set; }


        //Connections
        public string? UserProductId { get; set; } 
        public User User { get; set; } 
        public List<Cart> Carts { get; set; } 
        public List<Product> Products { get; set; } 
    }
}
