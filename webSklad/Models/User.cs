using Microsoft.AspNetCore.Identity;

namespace webSklad.Models
{
    public class User : IdentityUser
    {
        public string? NameUser { get; set; }
        public string? SurnameUser { get; set; }
        public string? CreatedByUserId { get; set; }


        //Connections
        public List<ShopPostInfo> ShopPostInfos { get; set; }

    }
}
