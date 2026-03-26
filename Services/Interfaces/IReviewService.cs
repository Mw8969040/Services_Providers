using Smart_Platform.ViewModel;

namespace Smart_Platform.Services.Interfaces
{
    public interface IReviewService
    {
        Task AddReviewAsync(ReviewVM reviewVM, string CustomerId);
        Task UpdateReviewAsync(ReviewVM reviewVM, string userId);
        Task DeleteReviewAsync(int reviewId, string userId);
        Task<IEnumerable<ReviewVM>> GetReviewsByServiceAsync(int serviceId);
    }
}
