using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Admin.Queries
{
    public record GetProvidersQuery() : IRequest<IEnumerable<UserDto>>;
}
