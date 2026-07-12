namespace Thanh_Video_Labeler.Controllers.VideoAPI.Payload
{
    public class UpdateVideo
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
    }
}