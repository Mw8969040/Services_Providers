using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartPlatform.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPlatform.Infrastructure.Data.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAllAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            // Only seed if the database is empty
            if (await context.ServiceCategories.AnyAsync())
            {
                return;
            }

            // 1. Seed Categories
            var categories = new List<ServiceCategory>
            {
                new ServiceCategory { Name = "Plumbing", Description = "Professional plumbing and pipe repair services.", ImageUrl = "https://images.unsplash.com/photo-1585704032915-c3400ca199e7?auto=format&fit=crop&q=80&w=800" },
                new ServiceCategory { Name = "Electrical", Description = "Electrical wiring, repairs, and appliance maintenance.", ImageUrl = "https://images.unsplash.com/photo-1621905251189-08b45d6a269e?auto=format&fit=crop&q=80&w=800" },
                new ServiceCategory { Name = "Cleaning", Description = "Home, office, and deep cleaning services.", ImageUrl = "https://images.unsplash.com/photo-1581578731548-c64695cc6952?auto=format&fit=crop&q=80&w=800" }
            };

            await context.ServiceCategories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            // 2. Seed Providers
            var providerUsers = new List<ApplicationUser>
            {
                new ApplicationUser { UserName = "ahmed@plumber.com", Email = "ahmed@plumber.com", FullName = "Ahmed The Plumber", EmailConfirmed = true, PhoneNumber = "0100000001", IsActive = true },
                new ApplicationUser { UserName = "sara@cleaner.com", Email = "sara@cleaner.com", FullName = "Sara Cleaning", EmailConfirmed = true, PhoneNumber = "0100000002", IsActive = true }
            };

            foreach (var prov in providerUsers)
            {
                if (await userManager.FindByEmailAsync(prov.Email) == null)
                {
                    await userManager.CreateAsync(prov, "Password@123");
                    await userManager.AddToRoleAsync(prov, "Provider");
                }
            }

            var providers = await userManager.GetUsersInRoleAsync("Provider");
            var ahmed = providers.FirstOrDefault(u => u.Email == "ahmed@plumber.com");
            var sara = providers.FirstOrDefault(u => u.Email == "sara@cleaner.com");

            if (ahmed != null && !await context.ProviderProfiles.AnyAsync(p => p.UserId == ahmed.Id))
            {
                await context.ProviderProfiles.AddAsync(new ProviderProfile { UserId = ahmed.Id, ProviderName = "Ahmed", BusinessName = "Plumbing Masters", Description = "Expert pipe fixing.", YearsOfExperience = 10, Rating = 4.8, ProfilePictureUrl = "https://i.pravatar.cc/150?u=ahmed" });
            }
            if (sara != null && !await context.ProviderProfiles.AnyAsync(p => p.UserId == sara.Id))
            {
                await context.ProviderProfiles.AddAsync(new ProviderProfile { UserId = sara.Id, ProviderName = "Sara", BusinessName = "Sparkle Clean", Description = "Best cleaning in town.", YearsOfExperience = 5, Rating = 4.9, ProfilePictureUrl = "https://i.pravatar.cc/150?u=sara" });
            }
            await context.SaveChangesAsync();

            // 3. Seed Customers
            var customerUsers = new List<ApplicationUser>
            {
                new ApplicationUser { UserName = "omar@customer.com", Email = "omar@customer.com", FullName = "Omar Customer", EmailConfirmed = true, PhoneNumber = "0111111111", IsActive = true },
                new ApplicationUser { UserName = "nada@customer.com", Email = "nada@customer.com", FullName = "Nada Customer", EmailConfirmed = true, PhoneNumber = "0122222222", IsActive = true }
            };

            foreach (var cust in customerUsers)
            {
                if (await userManager.FindByEmailAsync(cust.Email) == null)
                {
                    await userManager.CreateAsync(cust, "Password@123");
                    await userManager.AddToRoleAsync(cust, "Customer");
                }
            }

            var customers = await userManager.GetUsersInRoleAsync("Customer");
            var omar = customers.FirstOrDefault(u => u.Email == "omar@customer.com");
            var nada = customers.FirstOrDefault(u => u.Email == "nada@customer.com");

            if (omar != null && !await context.CustomerProfiles.AnyAsync(p => p.UserId == omar.Id))
            {
                await context.CustomerProfiles.AddAsync(new CustomerProfile { UserId = omar.Id, FullName = "Omar Khaled", Address = "Cairo, Maadi", ProfilePictureUrl = "https://i.pravatar.cc/150?u=omar" });
            }
            if (nada != null && !await context.CustomerProfiles.AnyAsync(p => p.UserId == nada.Id))
            {
                await context.CustomerProfiles.AddAsync(new CustomerProfile { UserId = nada.Id, FullName = "Nada Ali", Address = "Giza, Dokki", ProfilePictureUrl = "https://i.pravatar.cc/150?u=nada" });
            }
            await context.SaveChangesAsync();

            // 4. Seed Services
            if (!await context.Services.AnyAsync() && ahmed != null && sara != null)
            {
                var plumbingCat = await context.ServiceCategories.FirstOrDefaultAsync(c => c.Name == "Plumbing");
                var cleaningCat = await context.ServiceCategories.FirstOrDefaultAsync(c => c.Name == "Cleaning");

                var services = new List<Service>
                {
                    new Service { Title = "Sink Repair", Description = "Fixing leaking sinks.", BasePrice = 150, IsAvailable = true, CategoryId = plumbingCat!.Id, ProviderId = ahmed.Id },
                    new Service { Title = "Pipe Installation", Description = "Installing new house pipes.", BasePrice = 500, IsAvailable = true, CategoryId = plumbingCat.Id, ProviderId = ahmed.Id },
                    new Service { Title = "Deep Home Cleaning", Description = "Full thorough home cleaning.", BasePrice = 300, IsAvailable = true, CategoryId = cleaningCat!.Id, ProviderId = sara.Id }
                };

                await context.Services.AddRangeAsync(services);
                await context.SaveChangesAsync();
            }

            // 5. Seed Service Requests & Reviews
            if (!await context.ServiceRequests.AnyAsync() && omar != null && ahmed != null)
            {
                var sinkService = await context.Services.FirstOrDefaultAsync(s => s.Title == "Sink Repair");
                if (sinkService != null)
                {
                    var request = new ServiceRequest
                    {
                        ServiceId = sinkService.Id,
                        CustomerId = omar.Id,
                        CustomerPhoneNumber = "0111111111",
                        CustomerAddress = "Cairo, Maadi",
                        Notes = "Come quickly please.",
                        TotalPrice = 150,
                        RequestDate = DateTime.Now.AddDays(-2),
                        requestStatus = RequestStatus.Completed
                    };

                    await context.ServiceRequests.AddAsync(request);
                    await context.SaveChangesAsync();

                    // Add Review
                    var review = new Review
                    {
                        ServiceRequestId = request.Id,
                        Rating = 5,
                        Comment = "Amazing and very fast service!",
                        ReviewDate = DateTime.Now.AddDays(-1)
                    };
                    await context.Reviews.AddAsync(review);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
