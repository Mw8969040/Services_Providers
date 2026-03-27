namespace SmartPlatform.Domain.Entities
{
    public class CustomerProfile : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        public string FullName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
