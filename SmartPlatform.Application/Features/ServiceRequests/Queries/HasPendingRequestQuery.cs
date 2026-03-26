using MediatR;

namespace SmartPlatform.Application.Features.ServiceRequests.Queries
{
    public record HasPendingRequestQuery(int ServiceId, string CustomerId) : IRequest<bool>;
}
