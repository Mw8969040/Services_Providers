namespace Smart_Platform.Models
{
    public class ServiceCategory : BaseEntity
    {
        public string Name { get; set; }   
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<Service> Services { get; set; } = new HashSet<Service>();
    }
}
