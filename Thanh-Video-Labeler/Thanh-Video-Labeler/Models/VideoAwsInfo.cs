using Thanh_Video_Labeler.Enums;

namespace Thanh_Video_Labeler.Models
{
    public class VideoAwsInfo : Entity
    {
        public string Case { get; set; } = string.Empty;
        public string StoreLocation { get; set; } = string.Empty;
        public string ServerID { get; set; } = string.Empty;
        public string RequestedBy { get; set; } = string.Empty;
        public string RequestedDate { get; set; } = string.Empty;
        public string DateOfIncident { get; set; } = string.Empty;
        public string SessionName { get; set; } = string.Empty;
        public string ChannelName { get; set; } = string.Empty;
        public string BackupTime { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Tester { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string DateDownloadFile { get; set; } = string.Empty;
        public string AWSlink { get; set; } = string.Empty;
        public VideoStatus VideoStatus { get; set; }
    }
}