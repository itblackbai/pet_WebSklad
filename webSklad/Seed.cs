using Microsoft.AspNetCore.Identity;
using webSklad.Data;

namespace webSklad
{
    public static class SeedData
    {
        public static void Initialize(WebSkladContext context, IServiceProvider serviceProvider)
        {
            InitializeRoles(context, serviceProvider);

        }

        private static void InitializeRoles(WebSkladContext context, IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!roleManager.Roles.Any())
            {
                var rolesToAdd = new List<IdentityRole>
                {
                    new IdentityRole
                    {
                        Id = "29fcd8e0-9735-40e4-86de-ebec44cc87d7",
                        Name = "Cashier",
                        NormalizedName = "CASHIER"
                    },
                    new IdentityRole
                    {
                        Id = "ca2a37a3-a23b-4791-b7db-e4c701b58879",
                        Name = "Executor",
                        NormalizedName = "EXECUTOR"
                    },
                    new IdentityRole
                    {
                        Id = "d7cfd37c-0d58-47c5-8a96-cbfe804bd876",
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    },
                    new IdentityRole
                    {
                        Id = "daaebc8a-7edb-44b2-bbba-72c97a1dafa6",
                        Name = "Worker",
                        NormalizedName = "WORKER"
                    },
                    new IdentityRole
                    {
                        Id = "e53dd8c0-cb07-4e93-8a02-a2c7f23dc6e4",
                        Name = "ShopOwner",
                        NormalizedName = "SHOPOWNER"
                    },
                    new IdentityRole
                    {
                        Id = "f955c609-08ba-4c55-9f43-c101bb96066d",
                        Name = "Accountant",
                        NormalizedName = "ACCOUNTANT"
                    }
                };

                foreach (var role in rolesToAdd)
                {
                    roleManager.CreateAsync(role).GetAwaiter().GetResult();
                }


            }
        }
    }
}