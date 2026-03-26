using MediatR;
using X.PagedList;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Categories.Queries
{
    public record GetAllCategoriesQuery(int PageNumber = 1, int PageSize = 10) : IRequest<IPagedList<CategoryVM>>;
}
