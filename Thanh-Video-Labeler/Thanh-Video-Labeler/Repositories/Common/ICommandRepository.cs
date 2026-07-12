using Thanh_Video_Labeler.Models;

namespace Thanh_Video_Labeler.Repositories.Common
{
    public interface ICommandRepository<T> : IDisposable where T : Entity
    {
        Task<T> AddAsync(T entity);
        
        Task<T> UpdateAsync(T entity);
        
        Task<T> DeleteAsync(T entity);
        
        Task<ICollection<T>> AddRangeAsync(ICollection<T> entities);
        
        Task<ICollection<T>> UpdateRangeAsync(ICollection<T> entities);
        
        Task<bool> DeleteRangeAsync(ICollection<T> entities);
        
    }
}