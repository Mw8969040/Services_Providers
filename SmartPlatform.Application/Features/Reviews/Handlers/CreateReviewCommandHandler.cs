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

        public CreateReviewCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var serviceRequest = await _unitOfWork.Repository<ServiceRequest>().GetByIdWithIncludesAsync(r => r.Id == request.ReviewDto.ServiceRequestId, "Service");
            
            if (serviceRequest == null || serviceRequest.requestStatus != RequestStatus.Completed)
                throw new Exception("Review can only be added for completed requests");

            // Check if review already exists for this request
            var existingReview = await _unitOfWork.Repository<Review>()
                .GetByIdWithIncludesAsync(r => r.ServiceRequestId == request.ReviewDto.ServiceRequestId);

            if (existingReview != null)
            {
                throw new Exception("You have already reviewed this service request.");
            }

            var review = _mapper.Map<Review>(request.ReviewDto);
            await _unitOfWork.Repository<Review>().AddAsync(review);
            await _unitOfWork.CompleteAsync();

            // Invalidate Cache
            await _cacheService.RemoveAsync($"ServiceDetails_{serviceRequest.ServiceId}");
            await _cacheService.RemoveAsync($"ServiceRequests_List_P1_S10_Prall_Cu{serviceRequest.CustomerId}_Schnone_Bynone");
            await _cacheService.RemoveAsync($"DashboardStats_{serviceRequest.CustomerId}_Admin_False");
            await _cacheService.RemoveAsync($"DashboardStats_{serviceRequest.Service.ProviderId}_Admin_False");
            await _cacheService.RemoveAsync("DashboardStats_Admin_Global");

            // Recalculate Provider Rating
            var providerId = serviceRequest.Service.ProviderId;
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
