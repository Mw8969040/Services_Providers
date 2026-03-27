using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Categories.Queries;

namespace SmartPlatform.Application.Features.Categories.Handlers
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
    {
        private readonly IReadDbConnection _readDbConnection;

        public GetCategoryByIdQueryHandler(IReadDbConnection readDbConnection)
        {
            _readDbConnection = readDbConnection;
        }

        public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var sql = @"
                SELECT c.Id, c.Name, c.Description, c.ImageUrl,
                       (SELECT COUNT(*) FROM Services s WHERE s.CategoryId = c.Id AND s.IsDeleted = 0) as ServicesCount
                FROM ServiceCategories c
                WHERE c.Id = @Id AND c.IsDeleted = 0";

            return await _readDbConnection.QueryFirstOrDefaultAsync<CategoryDto>(sql, new { Id = request.Id });
        }
    }
}
