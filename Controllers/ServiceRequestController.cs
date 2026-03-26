using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smart_Platform.Services.Interfaces;
using Smart_Platform.ViewModel;

namespace Smart_Platform.Controllers
{
    public class ServiceRequestController : BaseController
    {
        private readonly IServiceRequestService _requestService;

        public ServiceRequestController(IServiceRequestService requestService)
        {
            _requestService = requestService;
        }

        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;
            var requests = await _requestService.GetRequestsForProviderAsync(userId, pageNumber, pageSize);
            return View(requests);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyRequests(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;
            var requests = await _requestService.GetRequestsForCustomerAsync(userId, pageNumber, pageSize);
            return View(requests);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int serviceId)
        {
            await _requestService.CreateAsync(serviceId, userId);
            TempData["Success"] = "Service requested successfully!";
            return RedirectToAction(nameof(MyRequests));
        }

        [Authorize(Roles = "Provider")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int requestId)
        {
            await _requestService.AcceptAsync(requestId, userId);
            TempData["Success"] = "Request accepted!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Provider")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int requestId)
        {
            await _requestService.RejectAsync(requestId, userId);
            TempData["Success"] = "Request rejected.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Provider")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int requestId)
        {
            await _requestService.CompleteAsync(requestId, userId);
            TempData["Success"] = "Request marked as completed!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int requestId)
        {
            await _requestService.CancelAsync(requestId, userId);
            TempData["Success"] = "Request cancelled.";
            return RedirectToAction(nameof(MyRequests));
        }
    }
}
