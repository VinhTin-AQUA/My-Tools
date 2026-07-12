using Thanh_Video_Labeler.DataContext;
using Thanh_Video_Labeler.Models;
using Thanh_Video_Labeler.Repositories.Common;

namespace Thanh_Video_Labeler.Repositories.ConfigRepository
{
    public interface IConfigCommandRepository : ICommandRepository<Config>
    {
        Task AddByOther();
    }
    
    public class ConfigCommandRepository(AppDbContext context)
        : CommandRepository<Config>(context), IConfigCommandRepository
    {
        public Task AddByOther()
        {
            throw new NotImplementedException();
        }
    }
}