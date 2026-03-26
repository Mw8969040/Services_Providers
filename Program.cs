using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Smart_Platform.Data;
using Smart_Platform.Models;
using Microsoft.AspNetCore.Identity;
using Smart_Platform.Data.Seed;
using Smart_Platform.Mapping;
using Smart_Platform.Services.Interfaces;
using Smart_Platform.UOW;
using Smart_Platform.Services.Implementation;

namespace Smart_Platform
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
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // MediatR
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

            // FluentValidation
            builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

            // IMemoryCache
            builder.Services.AddMemoryCache();

            // Add services to the container.
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<Filters.GlobalExceptionFilter>();
            });
            builder.Services.AddRazorPages();

            // Unit Of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Application Services
            builder.Services.AddScoped<IServiceService, ServiceService>();
            builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();

            var app = builder.Build();

            using (var Scope = app.Services.CreateScope())
            {
                var ServiceProvider = Scope.ServiceProvider;

                var roleManager = ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                UserManager<ApplicationUser> userManager = ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

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
