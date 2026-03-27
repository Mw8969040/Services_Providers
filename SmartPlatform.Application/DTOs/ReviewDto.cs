using System;
using System.ComponentModel.DataAnnotations;

namespace SmartPlatform.Application.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int ServiceRequestId { get; set; }
        public string? ServiceTitle { get; set; }
        
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }
        
        [Required(ErrorMessage = "Comment is required")]
        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
        public string Comment { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
    }
}
