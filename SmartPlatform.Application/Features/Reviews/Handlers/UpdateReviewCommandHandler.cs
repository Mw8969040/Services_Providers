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
        private readonly ICacheService _cacheService;
        private readonly IReadDbConnection _readDbConnection;

        public UpdateReviewCommandHandler(
            IUnitOfWork unitOfWork, 
            ICacheService cacheService,
            IReadDbConnection readDbConnection)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _readDbConnection = readDbConnection;
        }

        public async Task Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _unitOfWork.Repository<Review>().GetByIdWithIncludesAsync(
                r => r.Id == request.ReviewDto.Id, 
                "ServiceRequest.Service"
            );

            if (review == null) 
                throw new KeyNotFoundException("Review not found.");
                
            if (review.ServiceRequest!.CustomerId != request.CustomerId) 
                throw new UnauthorizedAccessException("You are not authorized to update this review.");

            review.Rating = request.ReviewDto.Rating;
            review.Comment = request.ReviewDto.Comment;

            _unitOfWork.Repository<Review>().Update(review);
            await _unitOfWork.CompleteAsync();

            await _cacheService.RemoveAsync($"ServiceDetails_{review.ServiceRequest.ServiceId}", cancellationToken);
            await _cacheService.RemoveGroupAsync($"Reviews_Service_{review.ServiceRequest.ServiceId}", cancellationToken);
            await _cacheService.RemoveGroupAsync("DashboardStats", cancellationToken);

            var providerId = review.ServiceRequest.Service!.ProviderId;
            var averageRatingSql = @"
                SELECT AVG(CAST(r.Rating AS FLOAT))
                FROM Reviews r
                INNER JOIN ServiceRequests sr ON r.ServiceRequestId = sr.Id
                INNER JOIN Services s ON sr.ServiceId = s.Id
                WHERE s.ProviderId = @ProviderId AND r.IsDeleted = 0";

            var averageRating = await _readDbConnection.QueryFirstOrDefaultAsync<double?>(
                averageRatingSql, 
                new { ProviderId = providerId }
            );

            if (averageRating.HasValue)
            {
                var profiles = await _unitOfWork.Repository<ProviderProfile>().GetAllWithIncludesAsync(p => p.UserId == providerId);
                var profile = profiles.FirstOrDefault();

                if (profile != null)
                {
                    profile.Rating = averageRating.Value;
                    _unitOfWork.Repository<ProviderProfile>().Update(profile);
                    await _unitOfWork.CompleteAsync();
                }
            }
        }
    }
}
