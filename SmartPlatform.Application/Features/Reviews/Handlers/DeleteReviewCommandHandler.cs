using MediatR;
using SmartPlatform.Application.Common.Interfaces;
using SmartPlatform.Application.Features.Reviews.Commands;
using SmartPlatform.Domain.Entities;

namespace SmartPlatform.Application.Features.Reviews.Handlers
{
    public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteReviewCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            var review = await _unitOfWork.Repository<Review>().GetByIdWithIncludesAsync(r => r.Id == request.Id, "ServiceRequest");

            if (review == null) throw new Exception("Review not found");
            if (review.ServiceRequest.CustomerId != request.CustomerId) throw new UnauthorizedAccessException();

            _unitOfWork.Repository<Review>().Delete(review);
            await _unitOfWork.CompleteAsync();
        }
    }
}
