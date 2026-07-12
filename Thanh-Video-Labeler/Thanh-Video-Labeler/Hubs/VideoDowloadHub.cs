using Microsoft.AspNetCore.SignalR;
using Thanh_Video_Labeler.Models;

namespace Thanh_Video_Labeler.Hubs
{
    public interface IVideoDowloadHub
    {
        Task RecieveResultDownloadVideo(ResultDownloadVideo  result);

        Task SendDownloadFinish(bool isFinish);
    }
    
    public class VideoDowloadHub : Hub<IVideoDowloadHub>
    {
        public static string ConnectionId = "";
        
        public override Task OnConnectedAsync()
        {
            if (string.IsNullOrEmpty(ConnectionId))
            {
                ConnectionId  = Context.ConnectionId;
            }
            
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            ConnectionId = "";
            return base.OnDisconnectedAsync(exception);
        }
    }
}