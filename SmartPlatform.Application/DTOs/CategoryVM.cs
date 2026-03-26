using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SmartPlatform.Application.DTOs
{
    public class CategoryVM
    {
        public int Id { get; set; }

        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Services Count")]
        public int ServicesCount { get; set; }
    }
}
