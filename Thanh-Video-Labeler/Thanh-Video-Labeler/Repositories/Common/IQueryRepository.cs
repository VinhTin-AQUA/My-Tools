using Thanh_Video_Labeler.Models;
using Thanh_Video_Labeler.Repositories.Models;

namespace Thanh_Video_Labeler.Repositories.Common
{
    public interface IQueryRepository<T> where T : Entity
    {
        Task<T?> GetByIdAsync(int id);
        
        Task<IEnumerable<T>> GetAllAsync();
        
        Task<List<T>> FilterAsync(QueryOptions<T, T> options);
        
        Task<bool> ExistsAsync();
    }
}