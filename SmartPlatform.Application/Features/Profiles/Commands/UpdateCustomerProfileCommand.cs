using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Profiles.Commands
{
    public class UpdateCustomerProfileCommand : IRequest<bool>
    {
        public CustomerProfileDto Profile { get; }

        public UpdateCustomerProfileCommand(CustomerProfileDto profile)
        {
            Profile = profile;
        }
    }
}
