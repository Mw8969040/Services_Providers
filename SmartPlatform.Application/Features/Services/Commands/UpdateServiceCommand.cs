using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Services.Commands
{
    public class UpdateServiceCommand : IRequest
    {
        public int Id { get; set; }
        public ServiceVM ServiceVM { get; set; }
        public string ProviderId { get; set; }

        public UpdateServiceCommand(int id, ServiceVM serviceVM, string providerId)
        {
            Id = id;
            ServiceVM = serviceVM;
            ProviderId = providerId;
        }
    }
}
