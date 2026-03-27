using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SmartPlatform.Application.Features.Reviews.Commands;
using SmartPlatform.Application.Features.Reviews.Queries;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Web.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ReviewController : BaseController
    {
        private readonly IMediator _mediator;

        public ReviewController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> ReviewForm(int requestId)
        {
            ViewBag.RequestId = requestId;
            return PartialView("_ReviewForm");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int requestId, int rating, string comment)
        {
            ReviewDto reviewDto = new ReviewDto
            {
                ServiceRequestId = requestId,
                Rating = rating,
                Comment = comment,
            };

            await _mediator.Send(new CreateReviewCommand(reviewDto));
            
            TempData["Success"] = "Review submitted successfully!";
            return RedirectToAction("MyRequests", "ServiceRequest");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReviewDto reviewDto)
        {
            await _mediator.Send(new UpdateReviewCommand(reviewDto, userId));
            TempData["Success"] = "Review updated successfully!";
            return RedirectToAction("MyRequests", "ServiceRequest");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteReviewCommand(id, userId));
            TempData["Success"] = "Review deleted successfully!";
            return RedirectToAction("MyRequests", "ServiceRequest");
        }
    }
}
