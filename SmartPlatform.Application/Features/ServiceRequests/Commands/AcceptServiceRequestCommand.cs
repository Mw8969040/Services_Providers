using MediatR;

namespace SmartPlatform.Application.Features.ServiceRequests.Commands
{
    public record AcceptServiceRequestCommand(int RequestId, string ProviderId) : IRequest;
}
