using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace webSklad.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        public string? Sku { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? IncomingPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        public string? BarCode { get; set; }
        public string? BarCodeOwn { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SubTotalCart { get; set; }
        public string? TypeCart { get; set; }




        //Connections

        [ForeignKey("Product")]
        public int? ProductId { get; set; }
        public int CartId { get; internal set; }
        public virtual Product Product { get; set; }

        public int? CartRegisterId { get; set; } 
        public CartRegister CartRegister { get; set; } 

        public int? InvoiceId { get; set; } 
        public Invoice Invoice { get; set; } 
    }
}
