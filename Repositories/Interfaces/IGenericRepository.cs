using System.Linq.Expressions;
using X.PagedList;
namespace Smart_Platform.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T> GetByIdAsync(int id);
        public Task AddAsync(T entity);
        public void Update(T entity);
        public void Delete(T entity);
        public Task<T> GetByIdWithIncludesAsync(Expression<Func<T, bool>> predicate, params string[] includes);
        public Task<IEnumerable<T>> GetAllWithIncludesAsync(Expression<Func<T, bool>> predicate, params string[] includes);

       // public Task<IPagedList<T>> GetPagedAsync(int pageNumber, int pageSize);
        public Task<IPagedList<T>> GetPagedWithIncludesAsync(int pageNumber, int pageSize, Expression<Func<T, bool>> predicate , params string[] includes);
    }
}


