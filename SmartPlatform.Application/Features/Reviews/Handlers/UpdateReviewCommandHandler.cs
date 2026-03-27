using MediatR;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Reviews.Commands;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.Reviews.Handlers
{
    public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateReviewCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _unitOfWork.Repository<Review>().GetByIdWithIncludesAsync(r => r.Id == request.ReviewDto.Id, "ServiceRequest.Service");

            if (review == null) throw new Exception("Review not found");
            if (review.ServiceRequest.CustomerId != request.CustomerId) throw new UnauthorizedAccessException();

            review.Rating = request.ReviewDto.Rating;
            review.Comment = request.ReviewDto.Comment;

            _unitOfWork.Repository<Review>().Update(review);
            await _unitOfWork.CompleteAsync();

            // Recalculate Provider Rating
            var providerId = review.ServiceRequest.Service.ProviderId;
            var providerReviews = await _unitOfWork.Repository<Review>().GetAllWithIncludesAsync(
                r => r.ServiceRequest.Service.ProviderId == providerId,
                "ServiceRequest.Service"
            );

            if (providerReviews.Any())
            {
                var averageRating = providerReviews.Average(r => r.Rating);
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
}
