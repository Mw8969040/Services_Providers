namespace Smart_Platform.Models
{
    public class Service : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsAvailable { get; set; }
        public string ProviderId { get; set; }
        public ApplicationUser? Provider { get; set; }
        public int CategoryId { get; set; }
        public ServiceCategory? Category { get; set; }
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new HashSet<ServiceRequest>();

    }
}