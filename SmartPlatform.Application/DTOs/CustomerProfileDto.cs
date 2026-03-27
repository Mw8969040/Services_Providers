using System.ComponentModel.DataAnnotations;

namespace SmartPlatform.Application.DTOs
{
    public class CustomerProfileDto
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full Name must be between 2 and 100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters")]
        public string? Address { get; set; }

        [DataType(DataType.ImageUrl)]
        public string? ProfilePictureUrl { get; set; }
    }
}
