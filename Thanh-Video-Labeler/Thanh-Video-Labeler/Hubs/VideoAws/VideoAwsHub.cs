using Microsoft.AspNetCore.SignalR;

namespace Thanh_Video_Labeler.Hubs.VideoAws
{
    public interface IVideoAwsHub
    {
        Task RecieveProgress(string progress);
        Task RecieveDowloadingInfoVideo(string videoName);
        Task RecieveErrorVideo(string _case, string serverId, string link);
        Task RecieveIncreaseSucess(int totalSuccess);
    }
    
    public class VideoAwsHub : Hub<IVideoAwsHub>
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