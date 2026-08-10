using QuickTools.Core.Enums;

namespace QuickTools.Services.Models.Iloveimg
{
    public class UpscaleImage
    {
        
    }
    
    public class UpscaleImageRequest
    {
        public string Scale { get; set; } = "1";
        public List<UpscaleImageRequestItem> UpscaleImageRequestItems { get; set; } = [];
    }
    
    public class UpscaleImageRequestItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Base64 { get; set; } = string.Empty;
        public string LocalPath { get; set; } = string.Empty;
    }
    
    public class UpscaleImageResponseItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LocalPath { get; set; } = string.Empty;
        public EProcessingStatus ProcessingStatus { get; set; }
        public long Size { get; set; }
    }
}