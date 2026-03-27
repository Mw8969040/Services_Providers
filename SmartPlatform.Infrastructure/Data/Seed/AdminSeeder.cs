using Microsoft.AspNetCore.Identity;
using SmartPlatform.Domain.Entities;
using System.IO;
using System.Linq;

namespace SmartPlatform.Infrastructure.Data.Seed
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@system.com";
            var logPath = Path.Combine(Directory.GetCurrentDirectory(), "seeding_log.txt");

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                await File.AppendAllTextAsync(logPath, $"Creating Admin: {adminEmail}\n");
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Mohamed wael",
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    await File.AppendAllTextAsync(logPath, "Admin Created Successfully.\n");
                }
                else
                {
                    await File.AppendAllTextAsync(logPath, $"Error creating admin: {string.Join(", ", result.Errors.Select(e => e.Description))}\n");
                }
            }
        }
    }
}