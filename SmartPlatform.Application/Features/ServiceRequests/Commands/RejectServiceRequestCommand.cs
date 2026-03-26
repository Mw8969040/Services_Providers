using MediatR;

namespace SmartPlatform.Application.Features.ServiceRequests.Commands
{
    public record RejectServiceRequestCommand(int RequestId, string ProviderId) : IRequest;
}
