using Thanh_Video_Labeler.DataContext;
using Thanh_Video_Labeler.Models;
using Thanh_Video_Labeler.Repositories.Common;

namespace Thanh_Video_Labeler.Repositories.VideoInfoRepository
{
    public interface IVideoInfoCommandRepository : ICommandRepository<VideoInfo>
    {
        Task AddWithOther();
    }

    public class VideoInfoCommandRepository(AppDbContext context)
        : CommandRepository<VideoInfo>(context), IVideoInfoCommandRepository
    {
        public Task AddWithOther()
        {
            throw new NotImplementedException();
        }
    }
}