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

            return await _readDbConnection.QueryFirstOrDefaultAsync<ServiceDto>(sql, new { Id = request.Id, CustomerId = request.CustomerId });
        }
    }
}
