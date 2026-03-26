using MediatR;
using Smart_Platform.ViewModel;
using X.PagedList;

namespace Smart_Platform.Features.Categories.Queries.GetAllCategories
{
    // Query: What data do we want?
    public record GetAllCategoriesQuery(int PageNumber, int PageSize) : IRequest<IPagedList<CategoryVM>>;
}
