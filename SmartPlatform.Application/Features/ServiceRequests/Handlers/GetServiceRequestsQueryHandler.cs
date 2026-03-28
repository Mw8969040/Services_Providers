using MediatR;
using X.PagedList;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.ServiceRequests.Queries;

namespace SmartPlatform.Application.Features.ServiceRequests.Handlers
{
    public class GetServiceRequestsQueryHandler : IRequestHandler<GetServiceRequestsQuery, IPagedList<ServiceRequestDto>>
    {
        private readonly IReadDbConnection _readDbConnection;
        private readonly ICacheService _cacheService;

        public GetServiceRequestsQueryHandler(IReadDbConnection readDbConnection, ICacheService cacheService)
        {
            _readDbConnection = readDbConnection;
            _cacheService = cacheService;
        }

        public async Task<IPagedList<ServiceRequestDto>> Handle(GetServiceRequestsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"ServiceRequests_List_P{request.PageNumber}_S{request.PageSize}_Pr{request.ProviderId ?? "all"}_Cu{request.CustomerId ?? "all"}_Sch{request.SearchTerm ?? "none"}_By{request.SearchBy ?? "none"}";

            var cachedData = await _cacheService.GetAsync<IPagedList<ServiceRequestDto>>(cacheKey);
            if (cachedData != null) return cachedData;

            var offset = (request.PageNumber - 1) * request.PageSize;

            string searchCondition = "";
            var searchParam = string.IsNullOrWhiteSpace(request.SearchTerm) ? null : $"%{request.SearchTerm}%";
            
            if (searchParam != null)
            {
                switch (request.SearchBy?.ToLower())
                {
                    case "customer":
                        searchCondition = "AND u.FullName LIKE @Search";
                        break;
                    case "phone":
                        searchCondition = "AND u.PhoneNumber LIKE @Search";
                        break;
                    case "service":
                    default:
                        searchCondition = "AND s.Title LIKE @Search";
                        break;
                }
            }

            var itemsSql = $@"
                SELECT r.Id, r.RequestDate, r.requestStatus, r.TotalPrice, r.ServiceId, r.CustomerId,
                       s.Title as ServiceTitle, u.FullName as CustomerName, u.PhoneNumber as CustomerPhoneNumber,
                       cp.Address as CustomerAddress,
                       CASE WHEN EXISTS (SELECT 1 FROM Reviews rev WHERE rev.ServiceRequestId = r.Id AND rev.IsDeleted = 0) THEN 1 ELSE 0 END as IsReviewed
                FROM ServiceRequests r
                JOIN Services s ON r.ServiceId = s.Id
                JOIN AspNetUsers u ON r.CustomerId = u.Id
                LEFT JOIN CustomerProfiles cp ON u.Id = cp.UserId
                WHERE (@ProviderId IS NULL OR s.ProviderId = @ProviderId)
                  AND (@CustomerId IS NULL OR r.CustomerId = @CustomerId)
                  {searchCondition}
                  AND r.IsDeleted = 0
                ORDER BY r.RequestDate DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var countSql = $@"
                SELECT COUNT(*) 
                FROM ServiceRequests r
                JOIN Services s ON r.ServiceId = s.Id
                JOIN AspNetUsers u ON r.CustomerId = u.Id
                WHERE (@ProviderId IS NULL OR s.ProviderId = @ProviderId)
                  AND (@CustomerId IS NULL OR r.CustomerId = @CustomerId)
                  {searchCondition}
                  AND r.IsDeleted = 0;";

            var parameters = new { 
                Search = searchParam,
                ProviderId = request.ProviderId, 
                CustomerId = request.CustomerId,
                Offset = offset, 
                PageSize = request.PageSize 
            };

            var items = await _readDbConnection.QueryAsync<ServiceRequestDto>(itemsSql, parameters);
            var totalCount = await _readDbConnection.QuerySingleAsync<int>(countSql, parameters);

            var result = new StaticPagedList<ServiceRequestDto>(items, request.PageNumber, request.PageSize, totalCount);

            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }
    }
}
