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
        private readonly IReadDbConnection _readDbConnection;

        public DeleteReviewCommandHandler(
            IUnitOfWork unitOfWork, 
            ICacheService cacheService,
            IReadDbConnection readDbConnection)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _readDbConnection = readDbConnection;
        }

        public async Task Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _unitOfWork.Repository<Review>().GetByIdWithIncludesAsync(
                r => r.Id == request.Id, 
                "ServiceRequest.Service"
            );

            if (review == null) 
                throw new KeyNotFoundException("Review not found.");
                
            if (review.ServiceRequest!.CustomerId != request.CustomerId) 
                throw new UnauthorizedAccessException("You are not authorized to delete this review.");

            var providerId = review.ServiceRequest.Service!.ProviderId;

            _unitOfWork.Repository<Review>().Delete(review);
            await _unitOfWork.CompleteAsync();

            await _cacheService.RemoveAsync($"ServiceDetails_{review.ServiceRequest.ServiceId}", cancellationToken);
            await _cacheService.RemoveGroupAsync("ServiceRequests", cancellationToken);
            await _cacheService.RemoveGroupAsync("DashboardStats", cancellationToken);

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

            var profiles = await _unitOfWork.Repository<ProviderProfile>().GetAllWithIncludesAsync(p => p.UserId == providerId);
            var profile = profiles.FirstOrDefault();

            if (profile != null)
            {
                profile.Rating = averageRating ?? 0;
                _unitOfWork.Repository<ProviderProfile>().Update(profile);
                await _unitOfWork.CompleteAsync();
            }
        }
    }
}
