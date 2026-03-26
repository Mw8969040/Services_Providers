using MediatR;
using Microsoft.AspNetCore.Mvc;
using Smart_Platform.Features.Categories.Queries.GetAllCategories;
using Smart_Platform.Models;
using Smart_Platform.Services.Interfaces;
using System.Diagnostics;

namespace Smart_Platform.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMediator _mediator;
        private readonly IServiceService _serviceService;

        public HomeController(ILogger<HomeController> logger, IMediator mediator, IServiceService serviceService)
        {
            _logger = logger;
            _mediator = mediator;
            _serviceService = serviceService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Categories = await _mediator.Send(new GetAllCategoriesQuery(1, 10));
            ViewBag.LatestServices = await _serviceService.GetAllAsync(1, 6);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
