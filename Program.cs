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
using MediatR;

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

            builder.Services.AddAutoMapper(cfg => {
                cfg.AddMaps(typeof(MappingProfile).Assembly);
            });

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetServicesQuery).Assembly));

            builder.Services.AddValidatorsFromAssembly(typeof(GetServicesQuery).Assembly);

            builder.Services.AddMemoryCache();
            builder.Services.AddTransient<ICacheService, SmartPlatform.Infrastructure.Services.MemoryCacheService>();

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IReadDbConnection, ReadDbConnection>();

var app = builder.Build();

            app.UseMiddleware<SmartPlatform.Web.Middlewares.GlobalExceptionMiddleware>();

            using (var Scope = app.Services.CreateScope())
            {
                var ServiceProvider = Scope.ServiceProvider;

                var roleManager = ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var context = ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await context.Database.MigrateAsync();

                await RoleSeeder.SeedRolesAsync(roleManager);
                await AdminSeeder.SeedAdminAsync(userManager);
                await DatabaseSeeder.SeedAllAsync(context, userManager);
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
