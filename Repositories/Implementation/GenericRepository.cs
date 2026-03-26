using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Smart_Platform.Data;
using Smart_Platform.Models;
using Smart_Platform.Repositories.Interfaces;
using X.PagedList;
//using X.PagedList.EFCore;

namespace Smart_Platform.Repositories.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int Id)
        {
            return await _dbSet.FindAsync(Id);
        }

        public async Task AddAsync(T Entity)
        {
            await _dbSet.AddAsync(Entity);
        }

        public void Update(T Entity)
        {
            if (Entity is BaseEntity baseEntity)
            {
                baseEntity.UpdateAt = DateTime.Now;
            }
            _dbSet.Update(Entity);
        }

        public void Delete(T Entity)
        {
            if (Entity is BaseEntity baseEntity)
            {
                baseEntity.IsDeleted = true;
                baseEntity.UpdateAt = DateTime.Now;
                _dbSet.Update(Entity);
            }
            else
            {
                _dbSet.Remove(Entity);
            }
        }

        public async Task<T> GetByIdWithIncludesAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = _dbSet;
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllWithIncludesAsync(Expression<Func<T, bool>> predicate, params string[] includes)
        {
            IQueryable<T> query = _dbSet;
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.Where(predicate).ToListAsync();
        }

        public async Task<IPagedList<T>> GetPagedAsync(int pageNumber, int pageSize)
        {
            int totalCount = await _dbSet.CountAsync();
            var items = await _dbSet
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new StaticPagedList<T>(items, pageNumber, pageSize, totalCount);
        }

        public async Task<IPagedList<T>> GetPagedWithIncludesAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? predicate = null, params string[] includes)
        {
            IQueryable<T> query = _dbSet;
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            // return await query.ToPagedListAsync(pageNumber, pageSize);

            int totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new StaticPagedList<T>(items, pageNumber, pageSize, totalCount);
        }
    }
}
