using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Reviews.Queries;

namespace SmartPlatform.Application.Features.Reviews.Handlers
{
    public class GetReviewsByServiceQueryHandler : IRequestHandler<GetReviewsByServiceQuery, IEnumerable<ReviewDto>>
    {
        private readonly IReadDbConnection _readDbConnection;
        private readonly ICacheService _cacheService;

        public GetReviewsByServiceQueryHandler(IReadDbConnection readDbConnection, ICacheService cacheService)
        {
            _readDbConnection = readDbConnection;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<ReviewDto>> Handle(GetReviewsByServiceQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Reviews_ByService_{request.ServiceId}";

            return await _cacheService.GetOrCreateAsync<IEnumerable<ReviewDto>>(
                key: cacheKey,
                factory: async ct =>
                {
                    var sql = @"
                        SELECT r.Id, r.Rating, r.Comment, r.ServiceRequestId,
                               u.FullName as CustomerName, u.Id as CustomerId
                        FROM Reviews r
                        JOIN ServiceRequests sr ON r.ServiceRequestId = sr.Id
                        JOIN AspNetUsers u ON sr.CustomerId = u.Id
                        WHERE sr.ServiceId = @ServiceId AND r.IsDeleted = 0";

                    return await _readDbConnection.QueryAsync<ReviewDto>(sql, new { ServiceId = request.ServiceId });
                },
                absoluteExpiration: TimeSpan.FromMinutes(10),
                group: $"Reviews_Service_{request.ServiceId}",
                slidingExpiration: TimeSpan.FromMinutes(2),
                cancellationToken: cancellationToken);
        }
    }
}
