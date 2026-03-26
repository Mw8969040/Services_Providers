using Smart_Platform.ViewModel;
using X.PagedList;

namespace Smart_Platform.Services.Interfaces
{
    public interface IServiceService
    {
        public Task<IPagedList<ServiceVM>> GetAllAsync(int pageNumber, int pageSize);
        public Task<IPagedList<ServiceVM>> GetByProviderAsync(string ProviderId, int pageNumber, int pageSize);
        public Task<IPagedList<ServiceVM>> GetByCategoryAsync(int CategoryId, int pageNumber, int pageSize);
        public Task<ServiceVM?> GetByIdAsync(int Id);
        public Task CreateAsync(ServiceVM serviceVM, string ProviderId);
        public Task UpdateAsync(int Id, ServiceVM serviceVM, string ProviderId);
        public Task DeleteAsync(int Id, string ProviderId);
    }
}
