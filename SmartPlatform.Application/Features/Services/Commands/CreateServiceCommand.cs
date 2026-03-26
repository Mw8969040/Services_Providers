using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Services.Commands
{
    public class CreateServiceCommand : IRequest
    {
        public ServiceVM ServiceVM { get; set; }
        public string ProviderId { get; set; }

        public CreateServiceCommand(ServiceVM serviceVM, string providerId)
        {
            ServiceVM = serviceVM;
            ProviderId = providerId;
        }
    }
}
