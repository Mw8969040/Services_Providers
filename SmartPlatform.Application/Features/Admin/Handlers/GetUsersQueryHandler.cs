using MediatR;
using X.PagedList;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Admin.Queries;

namespace SmartPlatform.Application.Features.Admin.Handlers
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IPagedList<UserDto>>
    {
        private readonly IReadDbConnection _readDbConnection;

        public GetUsersQueryHandler(IReadDbConnection readDbConnection)
        {
            _readDbConnection = readDbConnection;
        }

        public async Task<IPagedList<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var offset = (request.PageNumber - 1) * request.PageSize;

            string searchCondition = "";
            var searchParam = string.IsNullOrWhiteSpace(request.SearchTerm) ? null : $"%{request.SearchTerm}%";
            
            if (searchParam != null)
            {
                switch (request.SearchBy?.ToLower())
                {
                    case "name":
                        searchCondition = "AND u.FullName LIKE @Search";
                        break;
                    case "email":
                        searchCondition = "AND u.Email LIKE @Search";
                        break;
                    case "phone":
                        searchCondition = "AND u.PhoneNumber LIKE @Search";
                        break;
                    default:
                        searchCondition = "AND u.FullName LIKE @Search";
                        break;
                }
            }

            var itemsSql = $@"
                SELECT u.Id, u.FullName, u.Email, u.PhoneNumber, u.IsActive
                FROM AspNetUsers u
                WHERE 1=1 {searchCondition}
                ORDER BY u.Id
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var countSql = $@"
                SELECT COUNT(*) FROM AspNetUsers u
                WHERE 1=1 {searchCondition};";

            var parameters = new { Search = searchParam, Offset = offset, PageSize = request.PageSize };

            var items = (await _readDbConnection.QueryAsync<UserDto>(itemsSql, parameters)).ToList();
            var totalCount = await _readDbConnection.QuerySingleAsync<int>(countSql, parameters);

            if (items.Any())
            {
                var userIds = items.Select(u => u.Id).ToList();
                var rolesSql = @"
                    SELECT ur.UserId, r.Name as RoleName
                    FROM AspNetUserRoles ur
                    JOIN AspNetRoles r ON ur.RoleId = r.Id
                    WHERE ur.UserId IN @UserIds";
                
                var rolesData = await _readDbConnection.QueryAsync<dynamic>(rolesSql, new { UserIds = userIds });
                
                var rolesLookup = rolesData
                    .GroupBy(x => (string)x.UserId)
                    .ToDictionary(g => g.Key, g => g.Select(x => (string)x.RoleName).ToList());

                foreach (var user in items)
                {
                    if (rolesLookup.TryGetValue(user.Id, out var roles))
                    {
                        user.Roles = roles;
                    }
                }
            }

            return new StaticPagedList<UserDto>(items, request.PageNumber, request.PageSize, totalCount);
        }
    }
}
