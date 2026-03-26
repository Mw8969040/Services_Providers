using AutoMapper;
using Smart_Platform.Models;
using Smart_Platform.Services.Interfaces;
using Smart_Platform.UOW;
using Smart_Platform.ViewModel;

namespace Smart_Platform.Services.Implementation
{
    public class ReviewService : IReviewService
    {
        IUnitOfWork _unitOfWork;
        IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddReviewAsync(ReviewVM reviewVM, string CustomerId)
        {
            var request = await _unitOfWork
            .Repository<ServiceRequest>()
            .GetByIdWithIncludesAsync(r => r.Id == reviewVM.ServiceRequestId, "Service");

            if (request == null)
                throw new Exception("Service request not found");

            if (request.CustomerId != CustomerId)
                throw new UnauthorizedAccessException();

            if (request.requestStatus != RequestStatus.Completed)
                throw new Exception("You can only review completed services");

            // Check if user already reviewed this service (across any request)
            var existingReviews = await _unitOfWork.Repository<Review>()
                .GetAllWithIncludesAsync(r => r.ServiceRequest.ServiceId == request.ServiceId && r.ServiceRequest.CustomerId == CustomerId, "ServiceRequest");
            
            if (existingReviews.Any())
                throw new Exception("You have already reviewed this service.");

            var review = _mapper.Map<Review>(reviewVM);
            await _unitOfWork.Repository<Review>().AddAsync(review);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateReviewAsync(ReviewVM reviewVM, string userId)
        {
            var review = await _unitOfWork.Repository<Review>()
                .GetByIdWithIncludesAsync(r => r.Id == reviewVM.Id, "ServiceRequest");

            if (review == null) throw new Exception("Review not found");
            if (review.ServiceRequest.CustomerId != userId) throw new UnauthorizedAccessException();

            review.Rating = reviewVM.Rating;
            review.Comment = reviewVM.Comment;

            _unitOfWork.Repository<Review>().Update(review);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteReviewAsync(int reviewId, string userId)
        {
            var review = await _unitOfWork.Repository<Review>()
                .GetByIdWithIncludesAsync(r => r.Id == reviewId, "ServiceRequest");

            if (review == null) throw new Exception("Review not found");
            if (review.ServiceRequest.CustomerId != userId) throw new UnauthorizedAccessException();

            _unitOfWork.Repository<Review>().Delete(review);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<ReviewVM>> GetReviewsByServiceAsync(int serviceId)
        {
            var reviews = await _unitOfWork.Repository<Review>()
                .GetAllWithIncludesAsync(r => r.ServiceRequest.ServiceId == serviceId, "ServiceRequest.Customer", "ServiceRequest.Service");

            var mappedReviews = reviews.Select(r => {
                var vm = _mapper.Map<ReviewVM>(r);
                vm.CustomerName = r.ServiceRequest.Customer.FullName;
                vm.CustomerId = r.ServiceRequest.CustomerId;
                return vm;
            });

            return mappedReviews;
        }

    }
}
