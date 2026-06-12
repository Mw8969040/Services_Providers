using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.Services.Queries;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Services.Handlers
{
    public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, ServiceDto?>
    {
        private readonly IReadDbConnection _readDbConnection;
        private readonly ICacheService _cacheService;

        public GetServiceByIdQueryHandler(IReadDbConnection readDbConnection, ICacheService cacheService)
        {
            _readDbConnection = readDbConnection;
            _cacheService = cacheService;
        }

        public async Task<ServiceDto?> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"ServiceDetails_{request.Id}";

            var serviceDto = await _cacheService.GetOrCreateAsync<ServiceDto?>(
                key: cacheKey,
                factory: async ct =>
                {
                    var sql = @"
                        SELECT s.Id, s.Title, s.Description, s.BasePrice, s.ImageUrl,
                               s.IsAvailable, s.CategoryId, s.ProviderId,
                               c.Name     AS CategoryName,
                               u.FullName AS ProviderName
                        FROM   Services s
                        LEFT JOIN ServiceCategories c ON s.CategoryId = c.Id
                        LEFT JOIN AspNetUsers       u ON s.ProviderId  = u.Id
                        WHERE  s.Id = @Id AND s.IsDeleted = 0";

                    var dto = await _readDbConnection.QueryFirstOrDefaultAsync<ServiceDto>(sql, new { Id = request.Id });
                    if (dto is null) return null;

                    var reviewsSql = @"
                        SELECT r.Id, r.Rating, r.Comment, r.ReviewDate AS CreatedAt,
                               r.ServiceRequestId,
                               u.FullName AS CustomerName,
                               u.Id       AS CustomerId
                        FROM   Reviews r
                        INNER JOIN ServiceRequests sr ON r.ServiceRequestId = sr.Id
                        INNER JOIN AspNetUsers      u ON sr.CustomerId       = u.Id
                        WHERE  sr.ServiceId = @ServiceId AND r.IsDeleted = 0";

                    var reviews = await _readDbConnection.QueryAsync<ReviewDto>(reviewsSql, new { ServiceId = request.Id });
                    dto.Reviews = reviews.ToList();

                    if (dto.Reviews.Count > 0)
                        dto.AverageRating = dto.Reviews.Average(r => r.Rating);

                    return dto;
                },
                absoluteExpiration: TimeSpan.FromMinutes(10),
                group: "ServiceDetails",
                slidingExpiration: TimeSpan.FromMinutes(2),
                cancellationToken: cancellationToken);

            if (serviceDto is not null && !string.IsNullOrEmpty(request.CustomerId))
            {
                var pendingSql = @"
                    SELECT CASE WHEN EXISTS (
                        SELECT 1 FROM ServiceRequests sr
                        WHERE  sr.ServiceId     = @ServiceId
                          AND  sr.CustomerId    = @CustomerId
                          AND  sr.RequestStatus = 0  -- Pending
                          AND  sr.IsDeleted     = 0
                    ) THEN 1 ELSE 0 END";

                serviceDto.HasPendingRequest = await _readDbConnection
                    .QueryFirstOrDefaultAsync<bool>(pendingSql, new { ServiceId = request.Id, CustomerId = request.CustomerId });
            }

            return serviceDto;
        }
    }
}
