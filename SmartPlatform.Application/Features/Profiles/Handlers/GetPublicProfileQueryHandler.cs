using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Profiles.Queries;

namespace SmartPlatform.Application.Features.Profiles.Handlers
{
    public class GetPublicProfileQueryHandler : IRequestHandler<GetPublicProfileQuery, ProviderProfileDto?>
    {
        private readonly IReadDbConnection _readDbConnection;

        public GetPublicProfileQueryHandler(IReadDbConnection readDbConnection)
        {
            _readDbConnection = readDbConnection;
        }

        public async Task<ProviderProfileDto?> Handle(GetPublicProfileQuery request, CancellationToken cancellationToken)
        {
            var sql = @"
                SELECT 
                    COALESCE(pp.ProviderName, u.FullName) AS ProviderName,
                    COALESCE(pp.BusinessName, 'General Service') AS BusinessName,
                    COALESCE(pp.Description, 'This professional has not set up their profile details yet.') AS Description,
                    pp.ProfilePictureUrl,
                    ISNULL(pp.Rating, 0) AS Rating,
                    ISNULL(pp.YearsOfExperience, 0) AS YearsOfExperience
                FROM AspNetUsers u
                INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                INNER JOIN AspNetRoles r ON ur.RoleId = r.Id AND r.NormalizedName = 'PROVIDER'
                LEFT JOIN ProviderProfiles pp ON u.Id = pp.UserId AND pp.IsDeleted = 0
                WHERE u.Id = @ProviderId";

            return await _readDbConnection.QueryFirstOrDefaultAsync<ProviderProfileDto>(sql, new { ProviderId = request.ProviderId });
        }
    }
}
