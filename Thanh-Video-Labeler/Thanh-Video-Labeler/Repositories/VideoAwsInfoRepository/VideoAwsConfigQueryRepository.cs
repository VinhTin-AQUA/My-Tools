using Thanh_Video_Labeler.DataContext;
using Thanh_Video_Labeler.Models;
using Thanh_Video_Labeler.Repositories.Common;

namespace Thanh_Video_Labeler.Repositories.VideoAwsInfoRepository
{
    public interface IVideoAwsInfoQueryRepository : IQueryRepository<VideoAwsInfo>
    {
    }

    public class VideoAwsInfoQueryRepository(AppDbContext context)
        : QueryRepository<VideoAwsInfo>(context), IVideoAwsInfoQueryRepository
    {
    }
}
