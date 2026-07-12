using Thanh_Video_Labeler.Hubs;
using Thanh_Video_Labeler.Hubs.VideoAws;
using Thanh_Video_Labeler.Models;
using Thanh_Video_Labeler.Services;

namespace Thanh_Video_Labeler.Extensions
{
    public static class ServiceExtension
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ConfigService>();
            services.AddSingleton<VideoService>();
            services.AddSingleton<FileService>();
            services.AddSingleton<VideoExcelService>();
            services.AddSingleton<VideoAwsService>();
            
            services.AddSingleton<VideoDownloadHubService>();
            services.AddSingleton<VideoAwsHubService>();

            services.AddSingleton<AwsConfig>(configuration.GetSection("AwsConfig").Get<AwsConfig>()!);
        }
    }
}