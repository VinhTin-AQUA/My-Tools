namespace Thanh_Video_Labeler.Controllers.Common
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public string Message { get; set; } = "";
    }
}