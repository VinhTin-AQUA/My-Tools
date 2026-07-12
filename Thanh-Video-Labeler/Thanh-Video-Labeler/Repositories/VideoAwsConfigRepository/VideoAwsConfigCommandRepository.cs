using Thanh_Video_Labeler.DataContext;
using Thanh_Video_Labeler.Models;
using Thanh_Video_Labeler.Repositories.Common;

namespace Thanh_Video_Labeler.Repositories.VideoAwsConfigRepository
{
    public interface IVideoAwsConfigCommandRepository : ICommandRepository<VideoAwsConfig>
    {
    }

    public class VideoAwsConfigCommandRepository(AppDbContext context)
        : CommandRepository<VideoAwsConfig>(context), IVideoAwsConfigCommandRepository
    {
    }
}
