using QuickTools.Core.Enums;

namespace QuickTools.Services.Models.Iloveimg
{
    public class UpscaleImage
    {
        
    }
    
    public class IloveImgUpscaleImageRequest
    {
        public string Scale { get; set; } = "1";
        public List<IloveImgUpscaleImageRequestItem> UpscaleImageRequestItems { get; set; } = [];
    }
    
    public class IloveImgUpscaleImageRequestItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Base64 { get; set; } = string.Empty;
        public string LocalPath { get; set; } = string.Empty;
    }
    
    public class IloveImgUpscaleImageResponseItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LocalPath { get; set; } = string.Empty;
        public EProcessingStatus ProcessingStatus { get; set; }
        public long Size { get; set; }
    }
}