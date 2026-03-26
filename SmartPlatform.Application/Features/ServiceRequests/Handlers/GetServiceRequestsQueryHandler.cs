using MediatR;
using X.PagedList;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.ServiceRequests.Queries;

namespace SmartPlatform.Application.Features.ServiceRequests.Handlers
{
    public class GetServiceRequestsQueryHandler : IRequestHandler<GetServiceRequestsQuery, IPagedList<ServiceRequestVM>>
    {
        private readonly IReadDbConnection _readDbConnection;

        public GetServiceRequestsQueryHandler(IReadDbConnection readDbConnection)
        {
            _readDbConnection = readDbConnection;
        }

        public async Task<IPagedList<ServiceRequestVM>> Handle(GetServiceRequestsQuery request, CancellationToken cancellationToken)
        {
            var offset = (request.PageNumber - 1) * request.PageSize;

            var itemsSql = @"
                SELECT r.Id, r.RequestDate, r.requestStatus, r.TotalPrice, r.ServiceId, r.CustomerId,
                       s.Title as ServiceTitle, u.FullName as CustomerName
                FROM ServiceRequests r
                JOIN Services s ON r.ServiceId = s.Id
                JOIN AspNetUsers u ON r.CustomerId = u.Id
                WHERE (@ProviderId IS NULL OR s.ProviderId = @ProviderId)
                  AND (@CustomerId IS NULL OR r.CustomerId = @CustomerId)
                  AND r.IsDeleted = 0
                ORDER BY r.RequestDate DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var countSql = @"
                SELECT COUNT(*) 
                FROM ServiceRequests r
                JOIN Services s ON r.ServiceId = s.Id
                WHERE (@ProviderId IS NULL OR s.ProviderId = @ProviderId)
                  AND (@CustomerId IS NULL OR r.CustomerId = @CustomerId)
                  AND r.IsDeleted = 0;";

            var parameters = new { 
                ProviderId = request.ProviderId, 
                CustomerId = request.CustomerId,
                Offset = offset, 
                PageSize = request.PageSize 
            };

            var items = await _readDbConnection.QueryAsync<ServiceRequestVM>(itemsSql, parameters);
            var totalCount = await _readDbConnection.QuerySingleAsync<int>(countSql, parameters);

            return new StaticPagedList<ServiceRequestVM>(items, request.PageNumber, request.PageSize, totalCount);
        }
    }
}
