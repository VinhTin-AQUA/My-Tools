using Thanh_Video_Labeler.DataContext;
using Thanh_Video_Labeler.Models;
using Thanh_Video_Labeler.Repositories.Common;

namespace Thanh_Video_Labeler.Repositories.ConfigRepository
{
    public interface IConfigQueryRepository : IQueryRepository<Config>
    {
        Task GetAllByOther();
    }
    
    public class ConfigQueryRepository(AppDbContext context)
        : QueryRepository<Config>(context), IConfigQueryRepository
    {
        public Task GetAllByOther()
        {
            throw new NotImplementedException();
        }
    }
}