using System.Text.Json.Serialization;

namespace QuickTools.Core.Models
{
    public class GoogleSheetModel
    {
    }
    
    public class GoogleSheetConfig
    {
        public string Key { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    
    public class GoogleSheetApiRequest<T>
    {
        [JsonPropertyName("action")] public string Action { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Id { get; set; }

        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }
    }

    public class GoogleSheetApiResult
    {
        [JsonPropertyName("success")] public bool Success { get; set; }
        [JsonPropertyName("id")] public int? Id { get; set; }
        [JsonPropertyName("message")] public string? Message { get; set; }
    }
}