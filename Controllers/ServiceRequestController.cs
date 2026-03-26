using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SmartPlatform.Application.Features.ServiceRequests.Commands;
using SmartPlatform.Application.Features.ServiceRequests.Queries;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Web.Controllers
{
    public class ServiceRequestController : BaseController
    {
        private readonly IMediator _mediator;

        public ServiceRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;
            var requests = await _mediator.Send(new GetServiceRequestsQuery(ProviderId: userId, PageNumber: pageNumber, PageSize: pageSize));
            return View(requests);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyRequests(int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;
            var requests = await _mediator.Send(new GetServiceRequestsQuery(CustomerId: userId, PageNumber: pageNumber, PageSize: pageSize));
            return View(requests);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int serviceId)
        {
            var serviceRequestVM = new ServiceRequestVM { ServiceId = serviceId, CustomerId = userId };
            await _mediator.Send(new CreateServiceRequestCommand(serviceRequestVM));
            TempData["Success"] = "Service requested successfully!";
            return RedirectToAction(nameof(MyRequests));
        }

        [Authorize(Roles = "Provider")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int requestId)
        {
            await _mediator.Send(new AcceptServiceRequestCommand(requestId, userId));
            TempData["Success"] = "Request accepted!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Provider")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int requestId)
        {
            await _mediator.Send(new RejectServiceRequestCommand(requestId, userId));
            TempData["Success"] = "Request rejected.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Provider")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int requestId)
        {
            await _mediator.Send(new CompleteServiceRequestCommand(requestId, userId));
            TempData["Success"] = "Request marked as completed!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int requestId)
        {
            await _mediator.Send(new CancelServiceRequestCommand(requestId, userId));
            TempData["Success"] = "Request cancelled.";
            return RedirectToAction(nameof(MyRequests));
        }
    }
}
