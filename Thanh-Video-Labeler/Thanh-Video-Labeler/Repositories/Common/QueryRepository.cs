using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Thanh_Video_Labeler.DataContext;
using Thanh_Video_Labeler.Models;
using Thanh_Video_Labeler.Repositories.Models;

namespace Thanh_Video_Labeler.Repositories.Common
{
    public class QueryRepository<T> : IQueryRepository<T> where T : Entity
    {
        private readonly AppDbContext context;
        private readonly DbSet<T> dbSet;

        public QueryRepository(AppDbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var entity = await dbSet.FindAsync(id);
            return entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }
        
        public async Task<List<T>> FilterAsync(QueryOptions<T, T> options)
        {
            IQueryable<T> query = dbSet;

            if (options.AsNoTracking)
                query = query.AsNoTracking();

            // if (options.AsSplitQuery)
            //     query = query.AsSplitQuery();
            if (options.Include != null)
                query = options.Include(query);

            if (options.Predicate is Expression<Func<T, bool>> predicate)
                query = query.Where(predicate);

            if (options.OrderBy != null)
                query = options.OrderBy(query);

            if (options.Skip.HasValue)
                query = query.Skip(options.Skip.Value);

            if (options.Take.HasValue)
                query = query.Take(options.Take.Value);

            if (options.Selector != null)
                query = query.Select(options.Selector);

            var r = await query.ToListAsync(options.CancellationToken);
            return r;
        }

        public async Task<bool> ExistsAsync()
        {
            var r = await dbSet.AnyAsync();
            return r;
        }
    }
}