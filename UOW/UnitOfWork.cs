using Smart_Platform.Repositories.Interfaces;
using Smart_Platform.Repositories.Implementation;
using Smart_Platform.Data;

namespace Smart_Platform.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public IServiceRepository ServiceRepo { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            ServiceRepo = new ServiceRepository(context);
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new GenericRepository<TEntity>(_context);
            }
            return (IGenericRepository<TEntity>)_repositories[type];
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
