using MediatR;

namespace SmartPlatform.Application.Features.Services.Commands
{
    public class DeleteServiceCommand : IRequest
    {
        public int Id { get; set; }
        public string ProviderId { get; set; }

        public DeleteServiceCommand(int id, string providerId)
        {
            Id = id;
            ProviderId = providerId;
        }
    }
}
