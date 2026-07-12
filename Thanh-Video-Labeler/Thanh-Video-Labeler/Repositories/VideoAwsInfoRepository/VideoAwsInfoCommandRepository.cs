using Thanh_Video_Labeler.DataContext;
using Thanh_Video_Labeler.Models;
using Thanh_Video_Labeler.Repositories.Common;

namespace Thanh_Video_Labeler.Repositories.VideoAwsInfoRepository
{
    public interface IVideoAwsInfoCommandRepository : ICommandRepository<VideoAwsInfo>
    {
    }

    public class VideoAwsInfoCommandRepository(AppDbContext context)
        : CommandRepository<VideoAwsInfo>(context), IVideoAwsInfoCommandRepository
    {
    }
}
