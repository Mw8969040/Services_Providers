using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Smart_Platform.Models;

namespace Smart_Platform.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> Options) : base(Options) { }

        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.Customer)
                .WithMany(u => u.Requests)
                .HasForeignKey(sr => sr.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Service>()
                .HasOne(s => s.Provider)
                .WithMany(u => u.Services)
                .HasForeignKey(s => s.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Service>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<ServiceCategory>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<ServiceRequest>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Review>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
