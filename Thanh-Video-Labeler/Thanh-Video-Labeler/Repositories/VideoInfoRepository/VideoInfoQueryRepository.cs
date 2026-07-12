using Thanh_Video_Labeler.DataContext;
using Thanh_Video_Labeler.Enums;
using Thanh_Video_Labeler.Models;
using Thanh_Video_Labeler.Repositories.Common;

namespace Thanh_Video_Labeler.Repositories.VideoInfoRepository
{
    public interface IVideoInfoQueryRepository: IQueryRepository<VideoInfo>
    {
        Task<List<VideoInfo>> GetByStatus(VideoStatus status);
    }
    
    public class VideoInfoQueryRepository(AppDbContext context)
        : QueryRepository<VideoInfo>(context), IVideoInfoQueryRepository
    {
        public Task<List<VideoInfo>> GetByStatus(VideoStatus status)
        {
            throw new NotImplementedException();
        }
    }
}