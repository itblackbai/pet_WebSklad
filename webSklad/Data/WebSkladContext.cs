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
        public DbSet<CartRegister> CartRegisters { get; set; }
        public DbSet<Cart> CartItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SalesRepresentative> SalesRepresentatives { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<ShopPostInfo> ShopPostInfos { get; set; }
        public DbSet<Fop> FOPS { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.PostInfo)
                .WithMany()
                .HasForeignKey(i => i.PostInfoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.ShopInfo)
                .WithMany()
                .HasForeignKey(i => i.ShopInfoId)
                .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
