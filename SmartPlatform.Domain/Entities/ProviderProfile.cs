namespace SmartPlatform.Domain.Entities
{
    public class ProviderProfile : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;
        
        public string BusinessName { get; set; } = string.Empty; // Used for Specialty
        public string? ProviderName { get; set; } // Real name of the provider
        public string? Description { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public double Rating { get; set; }
        public int YearsOfExperience { get; set; }
    }
}
