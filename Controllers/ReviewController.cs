using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smart_Platform.Services.Interfaces;
using Smart_Platform.ViewModel;
using Smart_Platform.Models;
using Smart_Platform.UOW;
using AutoMapper;

namespace Smart_Platform.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ReviewController : BaseController
    {
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ReviewController(IReviewService reviewService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _reviewService = reviewService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> ReviewForm(int requestId)
        {
            ViewBag.RequestId = requestId;
            return PartialView("_ReviewForm");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var review = await _unitOfWork.Repository<Review>().GetByIdAsync(id);
            if (review == null) return NotFound();
            
            var reviewVM = _mapper.Map<ReviewVM>(review);
            return PartialView("_ReviewForm", reviewVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int requestId, int rating, string comment)
        {
            ReviewVM reviewVM = new ReviewVM
            {
                ServiceRequestId = requestId,
                Rating = rating,
                Comment = comment,
            };

            await _reviewService.AddReviewAsync(reviewVM, userId);
            
            TempData["Success"] = "Review submitted successfully!";
            return RedirectToAction("MyRequests", "ServiceRequest");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReviewVM reviewVM)
        {
            await _reviewService.UpdateReviewAsync(reviewVM, userId);
            TempData["Success"] = "Review updated successfully!";
            return RedirectToAction("MyRequests", "ServiceRequest");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _reviewService.DeleteReviewAsync(id, userId);
            TempData["Success"] = "Review deleted successfully!";
            return RedirectToAction("MyRequests", "ServiceRequest");
        }
    }
}
