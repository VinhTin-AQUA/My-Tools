using Microsoft.AspNetCore.SignalR;
using Thanh_Video_Labeler.Models;

namespace Thanh_Video_Labeler.Hubs
{
    public class VideoDownloadHubService
    {
        private readonly IHubContext<VideoDowloadHub, IVideoDowloadHub> videoDowloadHub;

        public VideoDownloadHubService(IHubContext<VideoDowloadHub, IVideoDowloadHub>  videoDowloadHub)
        {
            this.videoDowloadHub = videoDowloadHub;
        }

        public async Task RecieveTotalVideo(ResultDownloadVideo resultDownloadVideo)
        {
           await videoDowloadHub.Clients.Client(VideoDowloadHub.ConnectionId)
                .RecieveResultDownloadVideo(resultDownloadVideo);
        }
        
        public async Task SendDownloadFinish(bool isFinish)
        {
            await videoDowloadHub.Clients.Client(VideoDowloadHub.ConnectionId)
                .SendDownloadFinish(isFinish);
        }
    }
}