using MediatR;
using X.PagedList;
using SmartPlatform.Domain.Entities;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Admin.Queries
{
    public record GetUsersQuery(string? SearchBy = null, string? SearchTerm = null, int PageNumber = 1, int PageSize = 10) : IRequest<IPagedList<UserDto>>;
}
