using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Profiles.Queries;

namespace SmartPlatform.Application.Features.Profiles.Handlers
{
    public class GetCustomerProfileQueryHandler : IRequestHandler<GetCustomerProfileQuery, CustomerProfileDto?>
    {
        private readonly IReadDbConnection _readDbConnection;

        public GetCustomerProfileQueryHandler(IReadDbConnection readDbConnection)
        {
            _readDbConnection = readDbConnection;
        }

        public async Task<CustomerProfileDto?> Handle(GetCustomerProfileQuery request, CancellationToken cancellationToken)
        {
            var sql = "SELECT * FROM CustomerProfiles WHERE UserId = @UserId AND IsDeleted = 0";
            return await _readDbConnection.QueryFirstOrDefaultAsync<CustomerProfileDto>(sql, new { UserId = request.UserId });
        }
    }
}
