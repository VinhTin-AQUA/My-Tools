using System.Text.Json.Serialization;

namespace QuickTools.Core.Models
{
    public class PasswordManager
    {
        
    }
    
    public class GoogleSheetPassword
    {
        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("accountName")] public string AccountName { get; set; } = string.Empty;

        [JsonPropertyName("userName")] public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("password")] public string Password { get; set; } = string.Empty;

        [JsonPropertyName("note")] public string Note { get; set; } = string.Empty;
    }
}