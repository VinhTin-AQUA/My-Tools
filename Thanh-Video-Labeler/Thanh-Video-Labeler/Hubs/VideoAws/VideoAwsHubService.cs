using Microsoft.AspNetCore.SignalR;

namespace Thanh_Video_Labeler.Hubs.VideoAws
{
    public class VideoAwsHubService
    {
        private readonly IHubContext<VideoAwsHub, IVideoAwsHub> videoAwsHub;

        public VideoAwsHubService(IHubContext<VideoAwsHub, IVideoAwsHub>  videoAwsHub)
        {
            this.videoAwsHub = videoAwsHub;
        }

        public async Task RecieveProgress(string progress)
        {
            await videoAwsHub.Clients.Client(VideoAwsHub.ConnectionId).RecieveProgress(progress);
        }

        public async Task RecieveDowloadingInfoVideo(string videoName)
        {
            await videoAwsHub.Clients.Client(VideoAwsHub.ConnectionId).RecieveDowloadingInfoVideo(videoName);
        }

        public async Task RecieveErrorVideo(string _case, string serverId, string link)
        {
            await videoAwsHub.Clients.Client(VideoAwsHub.ConnectionId).RecieveErrorVideo(_case, serverId, link);
        }

        public async Task RecieveIncreaseSucess(int totalSuccess)
        {
            await videoAwsHub.Clients.Client(VideoAwsHub.ConnectionId).RecieveIncreaseSucess(totalSuccess);
        }
    }
}