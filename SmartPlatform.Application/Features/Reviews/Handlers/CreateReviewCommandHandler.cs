using MediatR;
using AutoMapper;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Reviews.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.Reviews.Handlers
{
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IReadDbConnection _readDbConnection;

        public CreateReviewCommandHandler(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ICacheService cacheService,
            IReadDbConnection readDbConnection)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
            _readDbConnection = readDbConnection;
        }

        public async Task Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var serviceRequest = await _unitOfWork.Repository<ServiceRequest>().GetByIdWithIncludesAsync(
                r => r.Id == request.ReviewDto.ServiceRequestId, 
                "Service"
            );
            
            if (serviceRequest == null || serviceRequest.requestStatus != RequestStatus.Completed)
                throw new InvalidOperationException("Review can only be added for completed service requests.");

            var existingReview = await _unitOfWork.Repository<Review>()
                .GetByIdWithIncludesAsync(r => r.ServiceRequestId == request.ReviewDto.ServiceRequestId);

            if (existingReview != null)
                throw new InvalidOperationException("You have already reviewed this service request.");

            var review = _mapper.Map<Review>(request.ReviewDto);
            await _unitOfWork.Repository<Review>().AddAsync(review);
            await _unitOfWork.CompleteAsync();

            await _cacheService.RemoveAsync($"ServiceDetails_{serviceRequest.ServiceId}", cancellationToken);
            await _cacheService.RemoveGroupAsync($"Reviews_Service_{serviceRequest.ServiceId}", cancellationToken);
            await _cacheService.RemoveGroupAsync("ServiceRequests", cancellationToken);
            await _cacheService.RemoveGroupAsync("DashboardStats", cancellationToken);

            var providerId = serviceRequest.Service.ProviderId;
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
