using MediatR;
using X.PagedList;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.ServiceRequests.Queries
{
    public record GetServiceRequestsQuery(
        string? SearchBy = null,
        string? SearchTerm = null,
        string? ProviderId = null, 
        string? CustomerId = null, 
        int PageNumber = 1, 
        int PageSize = 10) : IRequest<IPagedList<ServiceRequestDto>>;
}
