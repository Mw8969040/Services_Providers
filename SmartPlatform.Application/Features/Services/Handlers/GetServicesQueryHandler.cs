using MediatR;
using X.PagedList;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.Services.Queries;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Services.Handlers
{
    public class GetServicesQueryHandler : IRequestHandler<GetServicesQuery, IPagedList<ServiceDto>>
    {
        private readonly IReadDbConnection _readDbConnection;
        private readonly ICacheService _cacheService;

        public GetServicesQueryHandler(IReadDbConnection readDbConnection, ICacheService cacheService)
        {
            _readDbConnection = readDbConnection;
            _cacheService = cacheService;
        }

        public async Task<IPagedList<ServiceDto>> Handle(GetServicesQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Services_List_P{request.PageNumber}_S{request.PageSize}" +
                           $"_C{request.CategoryId ?? 0}_Pr{request.ProviderId ?? "all"}";

            return await _cacheService.GetOrCreateAsync<IPagedList<ServiceDto>>(
                key: cacheKey,
                factory: async ct =>
                {
                    var offset     = (request.PageNumber - 1) * request.PageSize;
                    var parameters = new
                    {
                        CategoryId = request.CategoryId,
                        ProviderId = request.ProviderId,
                        Offset     = offset,
                        PageSize   = request.PageSize
                    };

                    var itemsSql = @"
                        SELECT s.Id, s.Title, s.Description, s.BasePrice, s.ImageUrl,
                               s.IsAvailable, s.CategoryId, s.ProviderId,
                               c.Name  AS CategoryName,
                               u.FullName AS ProviderName
                        FROM   Services s
                        LEFT JOIN ServiceCategories c ON s.CategoryId = c.Id
                        LEFT JOIN AspNetUsers       u ON s.ProviderId  = u.Id
                        WHERE  (@CategoryId IS NULL OR s.CategoryId = @CategoryId)
                          AND  (@ProviderId IS NULL OR s.ProviderId  = @ProviderId)
                          AND  s.IsDeleted = 0
                        ORDER BY s.Id
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

                    var countSql = @"
                        SELECT COUNT(*)
                        FROM   Services s
                        WHERE  (@CategoryId IS NULL OR s.CategoryId = @CategoryId)
                          AND  (@ProviderId IS NULL OR s.ProviderId  = @ProviderId)
                          AND  s.IsDeleted = 0;";

                    var items      = await _readDbConnection.QueryAsync<ServiceDto>(itemsSql, parameters);
                    var totalCount = await _readDbConnection.QuerySingleAsync<int>(countSql, parameters);

                    return new StaticPagedList<ServiceDto>(items, request.PageNumber, request.PageSize, totalCount);
                },
                absoluteExpiration: TimeSpan.FromMinutes(10),
                group: "Services",
                slidingExpiration: TimeSpan.FromMinutes(2),
                cancellationToken: cancellationToken);
        }
    }
}
