using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Thanh_Video_Labeler.Repositories.Models
{
    public class QueryOptions<T, TResult>
    {
        public Expression? Predicate { get; set; }
       
        public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; set; }
        
        public Func<IQueryable<T>, IIncludableQueryable<T, object>>? Include { get; set; }
        
        public Expression<Func<T, T>>? Selector { get; set; }
        
        public int? Skip { get; set; }
       
        public int? Take { get; set; }
        
        public bool AsNoTracking { get; set; } = false;
       
        public bool AsSplitQuery { get; set; } = false;
        
        public CancellationToken CancellationToken { get; set; } = default;
    }
}