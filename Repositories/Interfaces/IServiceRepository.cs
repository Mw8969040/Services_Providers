using Smart_Platform.Models;

namespace Smart_Platform.Repositories.Interfaces
{
    public interface IServiceRepository : IGenericRepository<Service>
    {
        Task<IEnumerable<Service>> GetServiceByProviderWithCategoryAsync(string ProviderId);

        Task<IEnumerable<Service>> GetServicesByCategory(int categoryId);
    }
}
