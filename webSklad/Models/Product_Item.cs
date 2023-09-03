using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace webSklad.Models
{
    public class Product_item
    {
        public Product Product { get; set; }

        public string? Sku { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? IncomingPrice { get; set; }



        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }
        public string? BarCode { get; set; }
        public string? BarCodeOwn { get; set; }

        public string? TypeCart { get; set; }

        public int? Quantity { get; set; }

        private decimal? _SubTotal;
        public decimal? SubTotal
        {
            get { return _SubTotal; }
            set { _SubTotal = Product.IncomingPrice * Quantity; }
        }
        private decimal? _SubTotalСart;
        public decimal? SubTotalCart
        {
            get { return _SubTotalСart; }
            set { _SubTotalСart = Product.Price * Quantity; }
        }

        [Column(TypeName = "decimal(18,2)")]
        private decimal? Amount
        {
            get { return Amount; }
            set { Amount = Product.Amount + Quantity; }
        }


        [ForeignKey("Invoice")]
        public int? InvoiceId { get; set; } 
        public Invoice Invoice { get; set; } 

        [ForeignKey("CartRegister")]
        public int? CartRegisterId { get; set; } 
        public CartRegister CartRegister { get; set; } 


    }
}
