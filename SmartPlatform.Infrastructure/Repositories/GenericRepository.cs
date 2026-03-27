using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SmartPlatform.Infrastructure.Data;
using SmartPlatform.Domain.Entities;
using SmartPlatform.Application.Common.Interfaces;
using X.PagedList;

namespace SmartPlatform.Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _context.Entry(entity).State = EntityState.Detached;
            }
            return entity;
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.IsDeleted = true;
                _dbSet.Update(entity);
            }
            else
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task<TEntity?> GetByIdWithIncludesAsync(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            IQueryable<TEntity> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<TEntity>> GetAllWithIncludesAsync(Expression<Func<TEntity, bool>> predicate, params string[] includes)
        {
            IQueryable<TEntity> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<IPagedList<TEntity>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var count = await _dbSet.CountAsync();
            var items = await _dbSet.AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new StaticPagedList<TEntity>(items, pageNumber, pageSize, count);
        }

        public async Task<IPagedList<TEntity>> GetPagedWithIncludesAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, params string[] includes)
        {
            IQueryable<TEntity> query = _dbSet;
            
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var count = await query.CountAsync();
            var items = await query.AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new StaticPagedList<TEntity>(items, pageNumber, pageSize, count);
        }
    }
}
