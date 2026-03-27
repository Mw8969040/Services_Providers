using MediatR;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Features.Dashboard.Queries
{
    public record GetDashboardStatsQuery(string UserId, bool IsAdmin) : IRequest<DashboardDataDto>;
}
