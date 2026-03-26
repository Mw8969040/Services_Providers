using System.ComponentModel.DataAnnotations;
using Smart_Platform.Models;

namespace Smart_Platform.ViewModel
{
    public class ServiceRequestVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Service is required")]
        [Display(Name = "Service")]
        public int ServiceId { get; set; }

        [Display(Name = "Service Title")]
        public string? ServiceTitle { get; set; }

        [Display(Name = "Customer")]
        public string? CustomerId { get; set; }

        [Display(Name = "Customer Name")]
        public string? CustomerName { get; set; }

        [Display(Name = "Request Date")]
        public DateTime RequestDate { get; set; }

        [Required(ErrorMessage = "Request status is required")]
        [Display(Name = "Request Status")]
        public RequestStatus requestStatus { get; set; }

        [Required(ErrorMessage = "Total price is required")]
        [Range(0, 1000000, ErrorMessage = "Total price must be between 0 and 1000000")]
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }
    }
}
