using MediatR;

namespace Smart_Platform.Features.Categories.Commands.DeleteCategory
{
    // Command: Action to delete a category
    public record DeleteCategoryCommand(int Id) : IRequest;
}
