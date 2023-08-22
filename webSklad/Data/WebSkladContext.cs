using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using webSklad.Models;

namespace webSklad.Data
{
    public class WebSkladContext : IdentityDbContext<User>
    {
        public WebSkladContext(DbContextOptions<WebSkladContext> options)
        : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<SalesRepresentative> SalesRepresentatives { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<ShopPostInfo> ShopPostInfos { get; set; }
        public DbSet<Fop> FOPS { get; set; }

    }
}
