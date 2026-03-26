using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.ServiceRequests.Commands;
using SmartPlatform.Application.Features.ServiceRequests.Queries;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.ServiceRequests.Handlers
{
    public class HasPendingRequestQueryHandler : IRequestHandler<HasPendingRequestQuery, bool>
    {
        private readonly IReadDbConnection _readDbConnection;

        public HasPendingRequestQueryHandler(IReadDbConnection readDbConnection)
        {
            _readDbConnection = readDbConnection;
        }

        public async Task<bool> Handle(HasPendingRequestQuery request, CancellationToken cancellationToken)
        {
            var sql = @"
                SELECT COUNT(1)
                FROM ServiceRequests
                WHERE ServiceId = @ServiceId 
                  AND CustomerId = @CustomerId 
                  AND requestStatus = @Status
                  AND IsDeleted = 0";

            var count = await _readDbConnection.QuerySingleAsync<int>(sql, new { 
                ServiceId = request.ServiceId, 
                CustomerId = request.CustomerId,
                Status = (int)RequestStatus.Pending
            });

            return count > 0;
        }
    }
}
