using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Categories.Queries
{
    public record GetCategoryByIdQuery(int Id) : IRequest<CategoryDto?>;
}
