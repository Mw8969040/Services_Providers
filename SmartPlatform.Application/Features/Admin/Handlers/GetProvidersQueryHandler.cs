using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Admin.Queries;

namespace SmartPlatform.Application.Features.Admin.Handlers
{
    public class GetProvidersQueryHandler : IRequestHandler<GetProvidersQuery, IEnumerable<UserDto>>
    {
        private readonly IReadDbConnection _readDbConnection;

        public GetProvidersQueryHandler(IReadDbConnection readDbConnection)
        {
            _readDbConnection = readDbConnection;
        }

        public async Task<IEnumerable<UserDto>> Handle(GetProvidersQuery request, CancellationToken cancellationToken)
        {
            var sql = @"
                SELECT u.Id, u.FullName, u.Email, u.PhoneNumber, u.ProfileImage, u.IsActive,
                       r.Name as RoleName
                FROM AspNetUsers u
                LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
                WHERE r.Name = 'Provider'";

            var users = await _readDbConnection.QueryAsync<dynamic>(sql);
            
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                ProfileImage = u.ProfileImage,
                IsActive = u.IsActive,
                Roles = new List<string> { u.RoleName }
            });
        }
    }
}
