using MediatR;
using Smart_Platform.ViewModel;

namespace Smart_Platform.Features.Categories.Queries.GetCategoryById
{
    // Query: Get a specific category by ID
    public record GetCategoryByIdQuery(int Id) : IRequest<CategoryVM?>;
}
