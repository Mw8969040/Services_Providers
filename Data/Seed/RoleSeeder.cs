using Microsoft.AspNetCore.Identity;
using Smart_Platform.Models;

namespace Smart_Platform.Data.Seed
{
    public static class RoleSeeder
    {
            public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
            {
                string[] roles = { "Admin", "Provider", "Customer" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }
        }
    }



