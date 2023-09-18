using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace webSklad.Models.ViewModels
{
    public class InvoiceViewModel
    {
        [Required(ErrorMessage = "Enter the invoice number")]
        [StringLength(50, ErrorMessage = "Invoice number must contain up to 50 characters")]
        [Display(Name = "Invoice number")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Select a date")]
        public DateTime? DateTime { get; set; }
        [Required(ErrorMessage = "Select a date")]
        public DateTime? DateTimeNow { get; set; }
        [Required(ErrorMessage = "Select a date")]
        public DateTime? DatePostponement { get; set; }

        [Required(ErrorMessage = "Select a supplier")]
        [Display(Name = "Supplier")]
        public int? PostInfoId { get; set; }  
        public SelectList? PostInfos{ get; set; }
        [Required(ErrorMessage = "Select a vendor fop")]
        [Display(Name = "Provider Fop")]
        public int? PostFopId { get; set; } 
        public SelectList? PostFops { get; set; } 

        [Required(ErrorMessage = "Select a merchant vendor")]
        [Display(Name = "Trade Vendor")]
        public int? PostSRId { get; set; } 
        public SelectList? PostSR { get; set; }


        [Required(ErrorMessage = "Select a store")]
        [Display(Name = "Shop")]
        public int? ShopInfoId { get; set; }  
        public SelectList? ShopInfos { get; set; }
        [Required(ErrorMessage = "Select a vendor fop")]
        [Display(Name = "Provider Fop")]
        public int? ShopFopId { get; set; }  
        public SelectList? ShopFops { get; set; }  

    }
}
