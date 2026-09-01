using QuickTools.Core.Enums;
using QuickTools.Core.Models;

namespace QuickTools.Core.DTOs.Icons
{
    public class IconDTO
    {
        
    }
    
    public class SearchIconRequest : PaginationRequest { }
    
    public class SearchIconResponse : PaginationResult<IconModel> { }

    public class AddIconRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public EIconType IconType { get; set; }
    }
    
    public class DeleteIconRequest
    {
        public string Id { get; set; } = string.Empty;
    }
    
    public class UpdateIconRequest
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}