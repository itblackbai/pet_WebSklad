namespace webSklad.Models
{
    public class SalesRepresentative
    {
        public int Id { get; set; }
        public string? SRName { get; set; }
        public string? SRSurname { get; set; }
        public string? SREmail { get; set; }
        public string? SRPhoneNumber { get; set; }

        


        //Connections
        public int? ShopPostInfoId { get; set; } 
        public ShopPostInfo ShopPostInfo { get; set; } 
    }
}
