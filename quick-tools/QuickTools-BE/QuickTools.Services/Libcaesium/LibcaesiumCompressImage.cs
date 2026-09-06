using QuickTools.Core.Enums;

namespace QuickTools.Services.Models.Libcaesium
{
    public class LibcaesiumCompressImage
    {
        
    }

    public class LibcaesiumCompressImageRequest
    {
        public uint Quality { get; set; }
        public List<LibcaesiumCompressImageRequestItem> CompressImageRequestItems { get; set; } = [];
    }

    public class LibcaesiumCompressImageRequestItem
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Base64 { get; set; } = "";
        public string LocalPath { get; set; } = "";
    }
    
    public class LibcaesiumCompressImageResponseItem
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public EProcessingStatus ProcessingStatus { get; set; }
        public string LocalPath { get; set; } = "";
        public long Size { get; set; }
    }
}