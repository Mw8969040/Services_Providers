using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SmartPlatform.Infrastructure.Data;
using SmartPlatform.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using SmartPlatform.Infrastructure.Data.Seed;
using SmartPlatform.Application.Mapping;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Infrastructure.UOW;
using SmartPlatform.Application.Features.Services.Queries;

namespace SmartPlatform.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });

            // AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // MediatR
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetServicesQuery).Assembly));

            // FluentValidation
            builder.Services.AddValidatorsFromAssembly(typeof(GetServicesQuery).Assembly);

            // IMemoryCache
            builder.Services.AddMemoryCache();

            // Add services to the container.
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<Filters.GlobalExceptionFilter>();
            });
            builder.Services.AddRazorPages();

            // Infrastructure Services
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IReadDbConnection, ReadDbConnection>();

            // MediatR Handlers take care of the business logic formerly held in Services.

            var app = builder.Build();

            using (var Scope = app.Services.CreateScope())
            {
                var ServiceProvider = Scope.ServiceProvider;

                var roleManager = ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                await RoleSeeder.SeedRolesAsync(roleManager);
                await AdminSeeder.SeedAdminAsync(userManager);
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            
            app.MapRazorPages();

            app.Run();
        }
    }
}
