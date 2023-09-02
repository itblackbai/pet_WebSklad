using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace webSklad.Models.ViewModels
{
    public class InvoiceViewModel
    {
        [Required(ErrorMessage = "Внесіть номер накладнї")]
        [StringLength(50, ErrorMessage = "№ накладної повинно містити до 50 символів")]
        [Display(Name = "№ накладної")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Виберіть дату")]
        public DateTime? DateTime { get; set; }
        [Required(ErrorMessage = "Виберіть дату")]
        public DateTime? DateTimeNow { get; set; }
        [Required(ErrorMessage = "Виберіть дату")]
        public DateTime? DatePostponement { get; set; }



        // Вибір фопа та постачальника
        [Required(ErrorMessage = "Виберіть постачальника")]
        [Display(Name = "Постачальник")]
        public int? PostInfoId { get; set; }  //  [ShopPostInfoId]  вибір постачальника
        public SelectList? PostInfos{ get; set; }
        [Required(ErrorMessage = "Виберіть фопа постачальника")]
        [Display(Name = "Фоп постачальника")]
        public int? PostFopId { get; set; }  // [FOPId]  вибір фопа постачальника
        public SelectList? PostFops { get; set; }  // FOPs

        [Required(ErrorMessage = "Виберіть торгового постачальника")]
        [Display(Name = "Торговий постачальника")]
        public int? PostSRId { get; set; }  // [FOPId]  вибір фопа постачальника
        public SelectList? PostSR { get; set; }  // FOPs


        // Вибір фопа та магазин
        [Required(ErrorMessage = "Виберіть магазин")]
        [Display(Name = "Магазин")]
        public int? ShopInfoId { get; set; }  //  [ShopPostInfoId]  вибір постачальника
        public SelectList? ShopInfos { get; set; }
        [Required(ErrorMessage = "Виберіть фопа постачальника")]
        [Display(Name = "Фоп постачальника")]
        public int? ShopFopId { get; set; }  // [FOPId]  вибір фопа постачальника
        public SelectList? ShopFops { get; set; }  // FOPs

    }
}
