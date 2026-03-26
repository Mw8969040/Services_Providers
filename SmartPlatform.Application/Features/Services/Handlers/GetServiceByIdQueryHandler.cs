using MediatR;
using AutoMapper;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.Services.Queries;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.Services.Handlers
{
    public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, ServiceVM?>
    {
        private readonly IReadDbConnection _readDbConnection;

        public GetServiceByIdQueryHandler(IReadDbConnection readDbConnection)
        {
            _readDbConnection = readDbConnection;
        }

        public async Task<ServiceVM?> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
        {
            var sql = @"
                SELECT s.Id, s.Title, s.Description, s.BasePrice, s.ImageUrl, s.IsAvailable, s.CategoryId, s.ProviderId,
                       c.Name as CategoryName, u.FullName as ProviderName
                FROM Services s
                LEFT JOIN ServiceCategories c ON s.CategoryId = c.Id
                LEFT JOIN AspNetUsers u ON s.ProviderId = u.Id
                WHERE s.Id = @Id AND s.IsDeleted = 0";

            return await _readDbConnection.QueryFirstOrDefaultAsync<ServiceVM>(sql, new { Id = request.Id });
        }
    }
}
