using Microsoft.EntityFrameworkCore;
using Smart_Platform.Data;
using Smart_Platform.Models;
using Smart_Platform.Repositories.Interfaces;

namespace Smart_Platform.Repositories.Implementation
{
    public class ServiceRepository : GenericRepository<Service>, IServiceRepository
    {
        public ServiceRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Service>> GetServiceByProviderWithCategoryAsync(string ProviderId)
        {
            return await _dbSet.Where(s => s.ProviderId == ProviderId).Include(s => s.Category).ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetServicesByCategory(int categoryId)
        {
            return await _dbSet.Where(s => s.CategoryId == categoryId).ToListAsync();
        }
    }
}
