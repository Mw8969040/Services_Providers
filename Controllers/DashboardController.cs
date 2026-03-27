using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Security.Claims;
using SmartPlatform.Application.Features.Dashboard.Queries;

namespace SmartPlatform.Web.Controllers
{
    [Authorize(Roles = "Admin,Provider")]
    public class DashboardController : Controller
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            bool isAdmin = User.IsInRole("Admin");

            var stats = await _mediator.Send(new GetDashboardStatsQuery(userId, isAdmin));

            return View(stats);
        }
    }
}
