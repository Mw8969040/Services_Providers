using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.Reviews.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.Reviews.Handlers
{
    public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public DeleteReviewCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _unitOfWork.Repository<Review>().GetByIdWithIncludesAsync(r => r.Id == request.Id, "ServiceRequest.Service");

            if (review == null) throw new Exception("Review not found");
            if (review.ServiceRequest.CustomerId != request.CustomerId) throw new UnauthorizedAccessException();

            var providerId = review.ServiceRequest.Service.ProviderId;

            _unitOfWork.Repository<Review>().Delete(review);
            await _unitOfWork.CompleteAsync();

            // Invalidate Cache
            await _cacheService.RemoveAsync($"ServiceDetails_{review.ServiceRequest.ServiceId}");
            await _cacheService.RemoveAsync($"DashboardStats_{review.ServiceRequest.CustomerId}_Admin_False");
            await _cacheService.RemoveAsync($"DashboardStats_{providerId}_Admin_False");
            await _cacheService.RemoveAsync("DashboardStats_Admin_Global");
            await _cacheService.RemoveAsync($"ServiceRequests_List_P1_S10_Prall_Cu{review.ServiceRequest.CustomerId}_Schnone_Bynone");

            // Recalculate Provider Rating
            var providerReviews = await _unitOfWork.Repository<Review>().GetAllWithIncludesAsync(
                r => r.ServiceRequest.Service.ProviderId == providerId,
                "ServiceRequest.Service"
            );

            double averageRating = 0;
            if (providerReviews.Any())
            {
                averageRating = providerReviews.Average(r => r.Rating);
            }

            var profiles = await _unitOfWork.Repository<ProviderProfile>().GetAllWithIncludesAsync(p => p.UserId == providerId);
            var profile = profiles.FirstOrDefault();

            if (profile != null)
            {
                profile.Rating = averageRating;
                _unitOfWork.Repository<ProviderProfile>().Update(profile);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}
