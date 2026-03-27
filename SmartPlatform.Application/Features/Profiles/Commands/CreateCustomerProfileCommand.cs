using MediatR;

namespace SmartPlatform.Application.Features.Profiles.Commands
{
    public class CreateCustomerProfileCommand : IRequest<bool>
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
