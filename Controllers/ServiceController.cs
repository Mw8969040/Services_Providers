using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Categories.Queries;
using SmartPlatform.Application.Features.Services.Commands;
using SmartPlatform.Application.Features.Services.Queries;


namespace SmartPlatform.Web.Controllers
{

    [Authorize(Roles ="Provider")]
    public class ServiceController : BaseController
    {
        private readonly IMediator _mediator;

        public ServiceController(IMediator mediator)
        {
            _mediator = mediator;
        }


        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 6;
            var services = await _mediator.Send(new GetServicesQuery(pageNumber, pageSize, providerId: userId));
            return View(services);
        }

        public async Task<IActionResult> Create()
        {
            await LoadCategories();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceDto serviceDto)
        {
            if(!ModelState.IsValid)
            {
                await LoadCategories();
                return View(serviceDto);
            }

            await _mediator.Send(new CreateServiceCommand(serviceDto, userId));
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int serviceId)
        {
            var service = await _mediator.Send(new GetServiceByIdQuery(serviceId, userId));
            if (service == null)
                return NotFound();

            await LoadCategories(service.CategoryId);
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceDto service, int CategoryId)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategories(CategoryId);
                return View(service);
            }

            await _mediator.Send(new UpdateServiceCommand(service.Id, service, userId));
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int serviceId)
        {
            await _mediator.Send(new DeleteServiceCommand(serviceId, userId));
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetByCategory(int categoryId, int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 6;
            var services = await _mediator.Send(new GetServicesQuery(pageNumber, pageSize, categoryId: categoryId));
            return View(services);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var service = await _mediator.Send(new GetServiceByIdQuery(id, userId));
            if (service == null)
                return NotFound();
            
            return View(service);
        }

        private async Task LoadCategories(int? SelectedId = null)
        {
            var categories = await _mediator.Send(new GetAllCategoriesQuery(1, 100));
            ViewBag.categories = new SelectList(categories,"Id","Name",SelectedId);
        }
    }
}
