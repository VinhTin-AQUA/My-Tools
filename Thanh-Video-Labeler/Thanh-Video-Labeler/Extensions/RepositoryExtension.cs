using Thanh_Video_Labeler.Repositories.ConfigRepository;
using Thanh_Video_Labeler.Repositories.SheetRepository;
using Thanh_Video_Labeler.Repositories.VideoAwsConfigRepository;
using Thanh_Video_Labeler.Repositories.VideoAwsInfoRepository;
using Thanh_Video_Labeler.Repositories.VideoInfoRepository;

namespace Thanh_Video_Labeler.Extensions
{
    public static class RepositoryExtension
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IVideoInfoCommandRepository, VideoInfoCommandRepository>();
            services.AddScoped<IVideoInfoQueryRepository, VideoInfoQueryRepository>();

            services.AddScoped<IConfigCommandRepository, ConfigCommandRepository>();
            services.AddScoped<IConfigQueryRepository, ConfigQueryRepository>();

            services.AddScoped<ISheetCommandRepository, SheetCommandRepository>();
            services.AddScoped<ISheetQueryRepository, SheetQueryRepository>();
            
            services.AddScoped<IVideoAwsConfigCommandRepository, VideoAwsConfigCommandRepository>();
            services.AddScoped<IVideoAwsConfigQueryRepository, VideoAwsConfigQueryRepository>();
            
            services.AddScoped<IVideoAwsInfoCommandRepository, VideoAwsInfoCommandRepository>();
            services.AddScoped<IVideoAwsInfoQueryRepository, VideoAwsInfoQueryRepository>();
        }
    }
}