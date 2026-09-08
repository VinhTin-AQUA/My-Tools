using System.Text.Json.Serialization;

namespace QuickTools.Core.DTOs
{
    public class PaginationRequest
    {
        [JsonPropertyName("keyword")]
        public string? Keyword { get; set; }
    
        [JsonPropertyName("page")]
        public int Page { get; set; } = 1;
    
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; } = 10;
    }
    
    public class PaginationResult<T>
    {
        [JsonPropertyName("items")]
        public List<T> Items { get; set; } = new();
    
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
    
        [JsonPropertyName("page")]
        public int Page { get; set; }
    
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
    
        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }
    
        [JsonPropertyName("hasPreviousPage")]
        public bool HasPreviousPage => Page > 1;
    
        [JsonPropertyName("hasNextPage")]
        public bool HasNextPage => Page < TotalPages;
    
        // Helper methods
        public string GetPaginationInfo()
        {
            return $"Showing {Items.Count} of {TotalCount} items (Page {Page}/{TotalPages})";
        }
    }
}