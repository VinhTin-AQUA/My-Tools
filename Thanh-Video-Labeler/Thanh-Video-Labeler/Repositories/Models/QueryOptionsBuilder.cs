using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Thanh_Video_Labeler.Repositories.Models
{
    public class QueryOptionsBuilder<T> : IQueryOptionsBuilder<T> where T : class
    {
        private readonly QueryOptions<T, T> options = new();
        public QueryOptionsBuilder()
        {
            options.AsNoTracking = true;
        }
        
        public IQueryOptionsBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            options.Predicate = options.Predicate == null ? predicate :
                CombineWhereExpression(predicate);
            return this;
        }
       
        public IQueryOptionsBuilder<T> Include<TProperty>(
            Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            if (options.Include == null)
            {
                options.Include = q => (IIncludableQueryable<T,
                    object>)q.Include(navigationPropertyPath);
            }
            else
            {
                var previous = options.Include;
                options.Include = q => (IIncludableQueryable<T,
                    object>)previous(q).Include(navigationPropertyPath);
            }
            return this;
        }
       
        public IQueryOptionsBuilder<T> ThenInclude<TPreviousProperty, TProperty>(
            Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
        {
            if (options.Include == null)
                throw new InvalidOperationException("ThenInclude must follow an Include.");
            var previous = options.Include;
            options.Include = queryable =>
            {
                var includable = previous(queryable);
                return includable switch
                {
                    IIncludableQueryable<T, IEnumerable<TPreviousProperty>> collection =>
                        (IIncludableQueryable<T, object>)
                        collection.ThenInclude(navigationPropertyPath),
                    IIncludableQueryable<T, TPreviousProperty> reference => (IIncludableQueryable<T,
                        object>)reference
                        .ThenInclude(navigationPropertyPath),
                    _ => throw new InvalidOperationException("Invalid include chain.")
                };
            };
            return this;
        }

        public IQueryOptionsBuilder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (options.OrderBy == null)
                options.OrderBy = q => q.OrderBy(keySelector);
            else
                ThenBy(keySelector);
            return this;
        }
        
        public IQueryOptionsBuilder<T> OrderByDescending<TKey>(Expression<Func<T, TKey>>
            keySelector)
        {
            if (options.OrderBy == null)
                options.OrderBy = q => q.OrderByDescending(keySelector);
            else
                ThenByDescending(keySelector);
            return this;
        }
            
        public IQueryOptionsBuilder<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            if (options.OrderBy == null) throw new InvalidOperationException("ThenBy must follow an OrderBy.");
            var previous = options.OrderBy;
            options.OrderBy = q =>
            {
                var ordered = previous(q);
                return ordered.ThenBy(keySelector);
            };
            return this;
        }
        
        
        public IQueryOptionsBuilder<T> ThenByDescending<TKey>(Expression<Func<T, TKey>>
            keySelector)
        {
            if (options.OrderBy == null) throw new InvalidOperationException("ThenByDescending must follow an OrderBy.");
            var previous = options.OrderBy;
            options.OrderBy = q =>
            {
                var ordered = previous(q);
                return ordered.ThenByDescending(keySelector);
            };
            return this;
        }
        
        public IQueryOptionsBuilder<T> Select(Expression<Func<T, T>> selector)
        {
            options.Selector = selector;
            return this;
        }
        
        public IQueryOptionsBuilder<T> Skip(int skip)
        {
            options.Skip = skip;
            return this;
        }
        
        public IQueryOptionsBuilder<T> Take(int take)
        {
            options.Take = take;
            return this;
        }
        
        public IQueryOptionsBuilder<T> AsNoTracking(bool noTracking = true)
        {
            options.AsNoTracking = noTracking;
            return this;
        }
        
        public IQueryOptionsBuilder<T> AsSplitQuery()
        {
            options.AsSplitQuery = true;
            return this;
        }
        
        public IQueryOptionsBuilder<T> WithCancellationToken(CancellationToken token)
        {
            options.CancellationToken = token;
            return this;
        }
        
        public QueryOptions<T, T> Build()
        {
            return options;
        }
        
        private Expression<Func<T, bool>> CombineWhereExpression(Expression<Func<T, bool>>
            predicate)
        {
            if (options.Predicate == null)
                throw new InvalidOperationException("_options.Predicate must be set before combining.");
                
            var parameter = Expression.Parameter(typeof(T));
            var body = Expression.AndAlso(
                Expression.Invoke(options.Predicate, parameter),
                Expression.Invoke(predicate, parameter));
            var combined = Expression.Lambda<Func<T, bool>>(body, parameter);
            return combined;
        }
    }
}