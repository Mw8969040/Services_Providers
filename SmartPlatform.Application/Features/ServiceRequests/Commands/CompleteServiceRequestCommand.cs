using MediatR;

namespace SmartPlatform.Application.Features.ServiceRequests.Commands
{
    public record CompleteServiceRequestCommand(int RequestId, string ProviderId) : IRequest;
}
