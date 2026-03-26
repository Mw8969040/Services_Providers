using MediatR;

namespace SmartPlatform.Application.Features.ServiceRequests.Commands
{
    public record CancelServiceRequestCommand(int RequestId, string CustomerId) : IRequest;
}
