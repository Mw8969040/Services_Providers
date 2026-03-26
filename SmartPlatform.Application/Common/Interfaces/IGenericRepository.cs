using System.Linq.Expressions;
using X.PagedList;

namespace SmartPlatform.Application.Common.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<T> GetByIdWithIncludesAsync(Expression<Func<T, bool>> predicate, params string[] includes);
        Task<IEnumerable<T>> GetAllWithIncludesAsync(Expression<Func<T, bool>> predicate, params string[] includes);
        Task<IPagedList<T>> GetPagedWithIncludesAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? predicate = null, params string[] includes);
    }
}
