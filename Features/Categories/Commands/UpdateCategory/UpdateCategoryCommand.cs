using MediatR;
using Smart_Platform.ViewModel;

namespace Smart_Platform.Features.Categories.Commands.UpdateCategory
{
    // Command: Action to update a category
    public record UpdateCategoryCommand(CategoryVM CategoryVM) : IRequest;
}
