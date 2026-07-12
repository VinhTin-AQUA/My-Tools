using System.Linq.Expressions;

namespace Thanh_Video_Labeler.Repositories.Models
{
    public interface IQueryOptionsBuilder<T> where T : class
    {
        IQueryOptionsBuilder<T> Where(Expression<Func<T, bool>> predicate);
        IQueryOptionsBuilder<T> Include<TProperty>(Expression<Func<T, TProperty>>
            navigationPropertyPath);
        IQueryOptionsBuilder<T> ThenInclude<TPreviousProperty, TProperty>
            (Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath);
        IQueryOptionsBuilder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector);
        IQueryOptionsBuilder<T> OrderByDescending<TKey>(Expression<Func<T, TKey>>
            keySelector);
        IQueryOptionsBuilder<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector);
        IQueryOptionsBuilder<T> ThenByDescending<TKey>(Expression<Func<T, TKey>>
            keySelector);
        IQueryOptionsBuilder<T> Select(Expression<Func<T, T>> selector);
        IQueryOptionsBuilder<T> Skip(int skip);
        IQueryOptionsBuilder<T> Take(int take);
        IQueryOptionsBuilder<T> AsNoTracking(bool noTracking = true);
        IQueryOptionsBuilder<T> AsSplitQuery();
        IQueryOptionsBuilder<T> WithCancellationToken(CancellationToken token);
        QueryOptions<T, T> Build();
    }
}