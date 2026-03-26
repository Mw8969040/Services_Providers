using System.ComponentModel.DataAnnotations;

namespace Smart_Platform.ViewModel
{
    public class UserVM
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [MaxLength(150, ErrorMessage = "Full name cannot exceed 150 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Profile Image")]
        public string? ProfileImage { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Roles")]
        public List<string>? Roles { get; set; }
    }
}
