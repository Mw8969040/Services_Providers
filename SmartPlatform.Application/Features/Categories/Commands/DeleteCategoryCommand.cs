using MediatR;

namespace SmartPlatform.Application.Features.Categories.Commands
{
    public record DeleteCategoryCommand(int Id) : IRequest;
}
