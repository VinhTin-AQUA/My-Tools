using Thanh_Video_Labeler.DataContext;
using Thanh_Video_Labeler.Models;
using Thanh_Video_Labeler.Repositories.Common;

namespace Thanh_Video_Labeler.Repositories.VideoAwsConfigRepository
{
    public interface IVideoAwsConfigQueryRepository : IQueryRepository<VideoAwsConfig>
    {
    }

    public class VideoAwsConfigQueryRepository(AppDbContext context)
        : QueryRepository<VideoAwsConfig>(context), IVideoAwsConfigQueryRepository
    {
    }
}
