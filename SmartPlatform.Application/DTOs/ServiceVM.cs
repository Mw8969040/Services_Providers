using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SmartPlatform.Application.DTOs
{
    public class ServiceVM
    {
        public int Id { get; set; }

        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }

        [Required(ErrorMessage = "Service title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Service Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Service description is required")]
        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(1, 1000000, ErrorMessage = "Price must be between 1 and 1000000")]
        [Display(Name = "Base Price")]
        [DataType(DataType.Currency)]
        public decimal BasePrice { get; set; }

        [Display(Name = "Available")]
        public bool IsAvailable { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Category Name")]
        public string? CategoryName { get; set; }

        [Display(Name = "Provider")]
        public string? ProviderId { get; set; }

        [Display(Name = "Provider Name")]
        public string? ProviderName { get; set; }

        public bool HasPendingRequest { get; set; }
        public IEnumerable<ReviewVM> Reviews { get; set; } = new List<ReviewVM>();
    }
}
