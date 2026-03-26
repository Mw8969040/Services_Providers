using Smart_Platform.Models;
using Smart_Platform.ViewModel;
using X.PagedList;

namespace Smart_Platform.Services.Interfaces
{
    public interface IServiceRequestService
    {
        public Task CreateAsync(int ServiceId, string CustomerId);
        public Task AcceptAsync(int RequestId, string ProviderId);
        public Task RejectAsync(int RequestId, string ProviderId);
        public Task CompleteAsync(int RequestId, string ProviderId);
        public Task CancelAsync(int RequestId, string CustomerId);
        Task<IPagedList<ServiceRequestVM>> GetRequestsForProviderAsync(string providerId, int pageNumber, int pageSize);
        Task<IPagedList<ServiceRequestVM>> GetRequestsForCustomerAsync(string customerId, int pageNumber, int pageSize);
        Task<bool> HasPendingRequestAsync(int serviceId, string customerId);
    }
}
