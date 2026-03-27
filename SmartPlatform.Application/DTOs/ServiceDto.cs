using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SmartPlatform.Application.DTOs
{
    public class ServiceDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Base Price is required")]
        [Range(0, 1000000, ErrorMessage = "Price must be a positive value")]
        [Display(Name = "Base Price")]
        public decimal BasePrice { get; set; }

        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public string? ProviderId { get; set; }
        public string? ProviderName { get; set; }

        // Reviews
        public List<ReviewDto>? Reviews { get; set; } = new();
        public double AverageRating { get; set; }
        public bool HasPendingRequest { get; set; }
    }
}
