namespace QuickTools.Core.Responses
{
    public class WebUIResponse<T>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public bool Success { get; set; }
        public T? Data { get; set; }
    }
}