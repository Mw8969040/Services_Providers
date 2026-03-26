namespace SmartPlatform.Domain.Entities
{
    public class ServiceRequest : BaseEntity
    {
        public string CustomerId { get; set; }
        public ApplicationUser Customer { get; set; }

        public int ServiceId { get; set; }
        public Service Service {  get; set; }

        public DateTime RequestDate { get; set; }
        public RequestStatus requestStatus { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
