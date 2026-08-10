namespace QuickTools.Core.Models
{
    public class FileInfoModel
    {
        public string Name { get; init; } = string.Empty;

        public string FullPath { get; init; } = string.Empty;

        public string Directory { get; init; } = string.Empty;

        public string Extension { get; init; } = string.Empty;

        public long Size { get; init; }

        public DateTime CreationTime { get; init; }

        public DateTime LastModifiedTime { get; init; }

        public DateTime LastAccessTime { get; init; }

        public bool IsReadOnly { get; init; }

        public bool Exists { get; init; }
    }
}