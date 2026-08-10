namespace QuickTools.Core.Notifications
{
    public class Notification<T>
    {
        public string Action { get; set; } = "";
        public bool IsSuccess { get; set; }
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";   
        public T Data { get; set; } = default!;
    }
}