using MediatR;
using Smart_Platform.ViewModel;

namespace Smart_Platform.Features.Categories.Commands.CreateCategory
{
    // Command: Action to create a category
    public record CreateCategoryCommand(CategoryVM CategoryVM) : IRequest;
}
