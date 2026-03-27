using System.ComponentModel.DataAnnotations;

namespace SmartPlatform.Application.DTOs
{
    public class ProviderProfileDto
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Business Name or Specialty is required")]
        [StringLength(150, ErrorMessage = "Business Name cannot exceed 150 characters")]
        [Display(Name = "Specialty / Business Name")]
        public string BusinessName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Provider Name cannot exceed 100 characters")]
        [Display(Name = "Provider Name")]
        public string? ProviderName { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [DataType(DataType.ImageUrl)]
        public string? ProfilePictureUrl { get; set; }

        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
        public double Rating { get; set; }

        [Range(0, 50, ErrorMessage = "Years of experience must be a realistic number (0-50)")]
        [Display(Name = "Years of Experience")]
        public int YearsOfExperience { get; set; }
    }
}
