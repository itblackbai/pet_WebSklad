using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace webSklad.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public string? Sku { get; set; }

        public string? BarCode { get; set; }
        public string? BarCodeOwn { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? IncomingPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        public int? InvoiceId { get; set; } 
        public Invoice Invoice { get; set; }

        public int? ShopPostInfoId { get; set; } 
        public ShopPostInfo ShopPostInfo { get; set; } 

        public string? UserProductId { get; set; } 
        public User User { get; set; } 

    }
}
