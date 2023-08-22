using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.WebRequestMethods;

namespace webSklad.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? DateTime { get; set; }
        public DateTime? DatePostponement { get; set; }
        public DateTime? DateTimeNow { get; set; }

        public bool? IsPayment { get; set; }



        //Connections
        // Post and Fop Id
        [ForeignKey("PostInfo")]
        public int? PostInfoId { get; set; } 
        public ShopPostInfo PostInfo { get; set; }
        
        [ForeignKey("PostFopId")]
        public int? PostFopId { get; set; } 
        public Fop PostFop { get; set; }

        [ForeignKey("PostSRId")]
        public int? PostSRId { get; set; }
        public SalesRepresentative PostSR { get; set; }

        // Post and Fop Id
        [ForeignKey("ShopInfo")]
        public int? ShopInfoId { get; set; }
        public ShopPostInfo ShopInfo { get; set; }

        [ForeignKey("ShopFopId")]
        public int? ShopFopId { get; set; }
        public Fop ShopFop { get; set; }
    }
}
