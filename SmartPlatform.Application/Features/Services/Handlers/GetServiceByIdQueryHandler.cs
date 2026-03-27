using MediatR;
using AutoMapper;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.Services.Queries;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.Services.Handlers
{
    public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, ServiceDto?>
    {
        private readonly IReadDbConnection _readDbConnection;

        public GetServiceByIdQueryHandler(IReadDbConnection readDbConnection)
        {
            _readDbConnection = readDbConnection;
        }

        public async Task<ServiceDto?> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
        {
            var sql = @"
                SELECT s.Id, s.Title, s.Description, s.BasePrice, s.ImageUrl, s.IsAvailable, s.CategoryId, s.ProviderId,
                       c.Name as CategoryName, u.FullName as ProviderName,
                       CASE WHEN EXISTS (
                           SELECT 1 FROM ServiceRequests sr 
                           WHERE sr.ServiceId = s.Id 
                           AND sr.CustomerId = @CustomerId 
                           AND sr.RequestStatus = 0 -- Pending
                           AND sr.IsDeleted = 0
                       ) THEN 1 ELSE 0 END as HasPendingRequest
                FROM Services s
                LEFT JOIN ServiceCategories c ON s.CategoryId = c.Id
                LEFT JOIN AspNetUsers u ON s.ProviderId = u.Id
                WHERE s.Id = @Id AND s.IsDeleted = 0";

            var serviceDto = await _readDbConnection.QueryFirstOrDefaultAsync<ServiceDto>(sql, new { Id = request.Id, CustomerId = request.CustomerId });

            if (serviceDto != null)
            {
                var reviewsSql = @"
                    SELECT r.Id, r.Rating, r.Comment, r.ReviewDate as CreatedAt, r.ServiceRequestId,
                           u.FullName as CustomerName, u.Id as CustomerId
                    FROM Reviews r
                    INNER JOIN ServiceRequests sr ON r.ServiceRequestId = sr.Id
                    INNER JOIN AspNetUsers u ON sr.CustomerId = u.Id
                    WHERE sr.ServiceId = @ServiceId AND r.IsDeleted = 0";

                var reviews = await _readDbConnection.QueryAsync<ReviewDto>(reviewsSql, new { ServiceId = request.Id });
                serviceDto.Reviews = reviews.ToList();
                
                if (serviceDto.Reviews.Any())
                {
                    serviceDto.AverageRating = serviceDto.Reviews.Average(r => r.Rating);
                }
            }

            return serviceDto;
        }
    }
}
