using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartPlatform.Application.Features.Categories.Queries;
using SmartPlatform.Application.Features.Services.Queries;
using SmartPlatform.Application.DTOs;
using System.Diagnostics;

namespace SmartPlatform.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMediator _mediator;

        public HomeController(ILogger<HomeController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Categories = await _mediator.Send(new GetAllCategoriesQuery(1, 10));
            ViewBag.LatestServices = await _mediator.Send(new GetServicesQuery(1, 6));
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorDto { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
