using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Profiles.Queries;

namespace SmartPlatform.Application.Features.Profiles.Handlers
{
    public class GetProviderProfileQueryHandler : IRequestHandler<GetProviderProfileQuery, ProviderProfileDto?>
    {
        private readonly IReadDbConnection _readDbConnection;

        public GetProviderProfileQueryHandler(IReadDbConnection readDbConnection)
        {
            _readDbConnection = readDbConnection;
        }

        public async Task<ProviderProfileDto?> Handle(GetProviderProfileQuery request, CancellationToken cancellationToken)
        {
            var sql = "SELECT * FROM ProviderProfiles WHERE UserId = @UserId AND IsDeleted = 0";
            return await _readDbConnection.QueryFirstOrDefaultAsync<ProviderProfileDto>(sql, new { UserId = request.UserId });
        }
    }
}
