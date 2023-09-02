using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using webSklad.Data;
using webSklad.Interfaces;
using webSklad.Models;
using webSklad.Repository;

namespace webSklad
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            string connection = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<WebSkladContext>(options => options.UseSqlServer(connection));

            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<WebSkladContext>();


            //Interfaces && Repository
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
            builder.Services.AddScoped<IFopRepository, FopRepository>();
            builder.Services.AddScoped<IShopPostInfoRepository, ShopPostInfoRepository>();
            builder.Services.AddScoped<ISRRepository, SRRepository>();
            builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();


            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 0;
                options.Password.RequiredUniqueChars = 0;
            });


            var app = builder.Build();

            //Seed data
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<WebSkladContext>();
                    SeedData.Initialize(context, services);
                }
                catch (Exception ex)
                {

                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "createShopPostInfo",
                pattern: "Settings/CreateShopPostInfo",
                defaults: new { controller = "ShopPostInfo", action = "CreateShopPostInfo" });

            app.Run();
        }
    }
}