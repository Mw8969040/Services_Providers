using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Smart_Platform.Filters;
using Smart_Platform.Services.Interfaces;
using Smart_Platform.ViewModel;
using Smart_Platform.Features.Categories.Queries.GetAllCategories;

namespace Smart_Platform.Controllers
{

    [Authorize(Roles ="Provider")]
    public class ServiceController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IServiceService _serviceService;
        private readonly IServiceRequestService _requestService;
        private readonly IReviewService _reviewService;

        public ServiceController(IMediator mediator , IServiceService serviceService, IServiceRequestService requestService, IReviewService reviewService)
        {
            _mediator = mediator;
            _serviceService = serviceService;
            _requestService = requestService;
            _reviewService = reviewService;
        }


        [CacheResourceFilter("ServicesIndex")]
        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 6;
            var service = await _serviceService.GetByProviderAsync(userId, pageNumber, pageSize);
            return View(service);
        }

        public async Task<IActionResult> Create()
        {
            await LoadCategories();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CacheInvalidationFilter("ServicesIndex", "ServicesCategory")]
        public async Task<IActionResult> Create(ServiceVM serviceVM)
        {
            if(!ModelState.IsValid)
            {
                await LoadCategories();
                return View(serviceVM);
            }

            await _serviceService.CreateAsync(serviceVM, userId);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int serviceId)
        {
            var service = await _serviceService.GetByIdAsync(serviceId);
            if (service == null)
                return NotFound();

            await LoadCategories(service.CategoryId);
            return View(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CacheInvalidationFilter("ServicesIndex", "ServicesCategory")]
        public async Task<IActionResult> Edit(ServiceVM service, int CategoryId)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategories(CategoryId);
                return View(service);
            }

            await _serviceService.UpdateAsync(service.Id, service, userId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CacheInvalidationFilter("ServicesIndex", "ServicesCategory")]
        public async Task<IActionResult> Delete(int serviceId)
        {
            await _serviceService.DeleteAsync(serviceId, userId);
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        [CacheResourceFilter("ServicesCategory")]
        public async Task<IActionResult> GetByCategory(int categoryId, int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 6;
            var services = await _serviceService.GetByCategoryAsync(categoryId, pageNumber, pageSize);
            return View(services);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var service = await _serviceService.GetByIdAsync(id);
            if (service == null)
                return NotFound();

            if (User.Identity.IsAuthenticated && User.IsInRole("Customer"))
            {
                service.HasPendingRequest = await _requestService.HasPendingRequestAsync(id, userId);
            }

            service.Reviews = await _reviewService.GetReviewsByServiceAsync(id);

            return View(service);
        }

        private async Task LoadCategories(int? SelectedId = null)
        {
            var categories = await _mediator.Send(new GetAllCategoriesQuery(1, 100));
            ViewBag.categories = new SelectList(categories,"Id","Name",SelectedId);
        }
    }
}
