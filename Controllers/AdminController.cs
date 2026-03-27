using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SmartPlatform.Application.Features.Admin.Queries;
using SmartPlatform.Application.Features.Admin.Commands;

namespace SmartPlatform.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(string? searchBy, string? searchTerm, int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            ViewBag.SearchBy = searchBy;
            ViewBag.SearchTerm = searchTerm;
            
            var pagedList = await _mediator.Send(new GetUsersQuery(searchBy, searchTerm, pageNumber, pageSize));
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_UserTablePartial", pagedList);
            }
            
            return View(pagedList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeProvider(string Id)
        {
            await _mediator.Send(new MakeProviderCommand(Id));
            return RedirectToAction("Index");
        }
    }
}
