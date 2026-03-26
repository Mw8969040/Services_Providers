using System.ComponentModel.DataAnnotations;

namespace Smart_Platform.ViewModel
{
    public class ReviewVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Service request is required")]
        [Display(Name = "Service Request")]
        public int ServiceRequestId { get; set; }

        [Display(Name = "Service Title")]
        public string? ServiceTitle { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        [Display(Name = "Rating")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Comment is required")]
        [MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        [Display(Name = "Comment")]
        public string Comment { get; set; }

        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
    }
}
