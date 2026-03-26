namespace SmartPlatform.Domain.Entities
{
    public class Review : BaseEntity
    {
        public int ServiceRequestId {  get; set; }
        public ServiceRequest? ServiceRequest { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
