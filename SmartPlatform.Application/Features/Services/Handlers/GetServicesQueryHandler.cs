using MediatR;
using AutoMapper;
using X.PagedList;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.Services.Queries;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Domain.Entities;

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
            var cacheKey = $"Services_List_P{request.PageNumber}_S{request.PageSize}_C{request.CategoryId ?? 0}_Pr{request.ProviderId ?? "all"}";
            
            var cachedData = await _cacheService.GetAsync<IPagedList<ServiceDto>>(cacheKey);
            if (cachedData != null) return cachedData;

            var offset = (request.PageNumber - 1) * request.PageSize;

            var parameters = new { 
                CategoryId = request.CategoryId, 
                ProviderId = request.ProviderId,
                Offset = offset,
                PageSize = request.PageSize
            };
            
            
            var itemsSql = @"
                SELECT s.Id, s.Title, s.Description, s.BasePrice, s.ImageUrl, s.IsAvailable, s.CategoryId, s.ProviderId,
                       c.Name as CategoryName, u.FullName as ProviderName
                FROM Services s
                LEFT JOIN ServiceCategories c ON s.CategoryId = c.Id
                LEFT JOIN AspNetUsers u ON s.ProviderId = u.Id
                WHERE (@CategoryId IS NULL OR s.CategoryId = @CategoryId)
                  AND (@ProviderId IS NULL OR s.ProviderId = @ProviderId)
                  AND (s.IsDeleted = 0)
                ORDER BY s.Id
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var countSql = @"
                SELECT COUNT(*) 
                FROM Services s 
                WHERE (@CategoryId IS NULL OR s.CategoryId = @CategoryId) 
                  AND (@ProviderId IS NULL OR s.ProviderId = @ProviderId) 
                  AND (s.IsDeleted = 0);";

            var items = await _readDbConnection.QueryAsync<ServiceDto>(itemsSql, parameters);
            var totalCount = await _readDbConnection.QuerySingleAsync<int>(countSql, parameters);

            var result = new StaticPagedList<ServiceDto>(items, request.PageNumber, request.PageSize, totalCount);
            
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));
            
            return result;
        }
    }
}
