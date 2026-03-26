using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Categories.Commands
{
    public record UpdateCategoryCommand(CategoryVM CategoryVM) : IRequest;
}
