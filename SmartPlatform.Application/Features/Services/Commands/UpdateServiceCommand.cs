using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Services.Commands
{
    public class UpdateServiceCommand : IRequest
    {
        public int Id { get; set; }
        public ServiceDto ServiceDto { get; set; }
        public string ProviderId { get; set; }

        public UpdateServiceCommand(int id, ServiceDto serviceDto, string providerId)
        {
            Id = id;
            ServiceDto = serviceDto;
            ProviderId = providerId;
        }
    }
}
