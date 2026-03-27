using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Profiles.Commands
{
    public class UpdateProviderProfileCommand : IRequest<bool>
    {
        public ProviderProfileDto Profile { get; }

        public UpdateProviderProfileCommand(ProviderProfileDto profile)
        {
            Profile = profile;
        }
    }
}
