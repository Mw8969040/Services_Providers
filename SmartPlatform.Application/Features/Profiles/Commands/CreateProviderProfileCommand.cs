using MediatR;

namespace SmartPlatform.Application.Features.Profiles.Commands
{
    public class CreateProviderProfileCommand : IRequest<bool>
    {
        public string UserId { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty; // Specialty
        public string? ProviderName { get; set; }
        public string? Description { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public int YearsOfExperience { get; set; }
    }
}
