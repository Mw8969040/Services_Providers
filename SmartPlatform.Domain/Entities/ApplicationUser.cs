using Microsoft.AspNetCore.Identity;

namespace SmartPlatform.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string? ProfileImage { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Service> Services { get; set; } = new HashSet<Service>();
        public ICollection<ServiceRequest> Requests { get; set; } = new HashSet<ServiceRequest>();
    }
}
