using MediatR;
using X.PagedList;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Services.Queries
{
    public class GetServicesQuery : IRequest<IPagedList<ServiceDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? CategoryId { get; set; }
        public string? ProviderId { get; set; }

        public GetServicesQuery(int pageNumber, int pageSize, int? categoryId = null, string? providerId = null)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            CategoryId = categoryId;
            ProviderId = providerId;
        }
    }
}
