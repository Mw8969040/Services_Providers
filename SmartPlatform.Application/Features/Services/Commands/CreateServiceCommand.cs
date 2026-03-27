using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Services.Commands
{
    public class CreateServiceCommand : IRequest
    {
        public ServiceDto ServiceDto { get; set; }
        public string ProviderId { get; set; }

        public CreateServiceCommand(ServiceDto serviceDto, string providerId)
        {
            ServiceDto = serviceDto;
            ProviderId = providerId;
        }
    }
}
