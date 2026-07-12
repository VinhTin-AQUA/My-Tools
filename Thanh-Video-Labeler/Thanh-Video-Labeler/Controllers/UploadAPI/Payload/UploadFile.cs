namespace Thanh_Video_Labeler.Controllers.UploadAPI.Payload
{
    public class UploadFile
    {
        public IFormFile? File { get; set; }
        public bool IsAccepted { get; set; }
    }
}