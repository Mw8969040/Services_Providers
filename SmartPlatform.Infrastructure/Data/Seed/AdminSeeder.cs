using Microsoft.AspNetCore.Identity;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Infrastructure.Data.Seed
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@system.com";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Mohamed wael",
                    EmailConfirmed = true,


                };

                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

        }
    }
}