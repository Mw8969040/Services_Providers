using Smart_Platform.Repositories.Interfaces;
using Smart_Platform.Models;

namespace Smart_Platform.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
        IServiceRepository ServiceRepo { get; }
        Task<int> CompleteAsync();
    }
}
