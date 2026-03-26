using MediatR;
using X.PagedList;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Admin.Queries;

namespace SmartPlatform.Application.Features.Admin.Handlers
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IPagedList<UserVM>>
    {
        private readonly IReadDbConnection _readDbConnection;

        public GetUsersQueryHandler(IReadDbConnection readDbConnection)
        {
            _readDbConnection = readDbConnection;
        }

        public async Task<IPagedList<UserVM>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var offset = (request.PageNumber - 1) * request.PageSize;

            var itemsSql = @"
                SELECT u.Id, u.FullName, u.Email, u.PhoneNumber, u.IsActive
                FROM AspNetUsers u
                ORDER BY u.Id
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var countSql = @"SELECT COUNT(*) FROM AspNetUsers;";

            var parameters = new { Offset = offset, PageSize = request.PageSize };

            var items = await _readDbConnection.QueryAsync<UserVM>(itemsSql, parameters);
            var totalCount = await _readDbConnection.QuerySingleAsync<int>(countSql, parameters);

            return new StaticPagedList<UserVM>(items, request.PageNumber, request.PageSize, totalCount);
        }
    }
}
