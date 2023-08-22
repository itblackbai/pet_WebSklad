using static System.Net.WebRequestMethods;

namespace webSklad.Models
{
    public class ShopPostInfo
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }




        //Connections
        public List<Fop> FOPS { get; set; }
        public List<SalesRepresentative> SalesRepresentatives { get; set; }


        public string? UserId { get; set; } 
        public User User { get; set; } 

    }
}
