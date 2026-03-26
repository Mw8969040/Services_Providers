using MediatR;
using X.PagedList;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Categories.Queries;

namespace SmartPlatform.Application.Features.Categories.Handlers
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IPagedList<CategoryVM>>
    {
        private readonly IReadDbConnection _readDbConnection;

        public GetAllCategoriesQueryHandler(IReadDbConnection readDbConnection)
        {
            _readDbConnection = readDbConnection;
        }

        public async Task<IPagedList<CategoryVM>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var offset = (request.PageNumber - 1) * request.PageSize;

            var itemsSql = @"
                SELECT c.Id, c.Name, c.Description, c.ImageUrl,
                       (SELECT COUNT(*) FROM Services s WHERE s.CategoryId = c.Id AND s.IsDeleted = 0) as ServicesCount
                FROM ServiceCategories c
                WHERE c.IsDeleted = 0
                ORDER BY c.Id
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

            var countSql = @"SELECT COUNT(*) FROM ServiceCategories WHERE IsDeleted = 0;";

            var parameters = new { Offset = offset, PageSize = request.PageSize };

            var items = await _readDbConnection.QueryAsync<CategoryVM>(itemsSql, parameters);
            var totalCount = await _readDbConnection.QuerySingleAsync<int>(countSql, parameters);

            return new StaticPagedList<CategoryVM>(items, request.PageNumber, request.PageSize, totalCount);
        }
    }
}
