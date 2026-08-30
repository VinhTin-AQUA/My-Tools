using QuickTools.Core.Models;

namespace QuickTools.Windows.Helpers
{
    public static class FilesHelper
    {
        public static async Task<string> SaveBase64File(string base64String, string fileName, EFolder folder)
        {
            try
            {
                string iloveimg_Upscale_Temp = FolderHelper.GetFolder(EFolder.Iloveimg_Upscale_Temp);
       
                // Tạo thư mục nếu chưa tồn tại (kể cả thư mục con)
                Directory.CreateDirectory(iloveimg_Upscale_Temp);
        
                // Xử lý base64 (loại bỏ phần header nếu có)
                string cleanBase64 = base64String;
                if (base64String.Contains(","))
                {
                    cleanBase64 = base64String.Substring(base64String.IndexOf(",") + 1);
                }
        
                // Chuyển đổi base64 sang byte array
                byte[] fileBytes = Convert.FromBase64String(cleanBase64);
        
                // Tạo đường dẫn file đầy đủ
                string filePath = Path.Combine(iloveimg_Upscale_Temp, fileName);
        
                // Lưu file
                await File.WriteAllBytesAsync(filePath, fileBytes);
        
                return filePath; // Trả về đường dẫn đã lưu
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu file: {ex.Message}");
            }
        }
        
        public static FileInfoModel GetInfo(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException(
                    "File path cannot be empty.",
                    nameof(filePath));

            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
            {
                return new FileInfoModel
                {
                    Name = fileInfo.Name,
                    FullPath = fileInfo.FullName,
                    Directory = fileInfo.DirectoryName ?? string.Empty,
                    Extension = fileInfo.Extension,
                    Exists = false
                };
            }

            return new FileInfoModel()
            {
                Name = fileInfo.Name,
                FullPath = fileInfo.FullName,
                Directory = fileInfo.DirectoryName ?? string.Empty,
                Extension = fileInfo.Extension,
                Size = fileInfo.Length,
                CreationTime = fileInfo.CreationTime,
                LastModifiedTime = fileInfo.LastWriteTime,
                LastAccessTime = fileInfo.LastAccessTime,
                IsReadOnly = fileInfo.IsReadOnly,
                Exists = true
            };
        }
        
        public static async Task<string> SaveFileAsync(
            byte[] bytes,
            string fileName,
            params string[] folders)
        {
            string folderPath = CombineAndCreateFolder(folders);
            string filePath = Path.Combine(folderPath, fileName);
            await File.WriteAllBytesAsync(filePath, bytes);
            return filePath;
        }
        
        private static string CombineAndCreateFolder(params string[] folders)
        {
            if (folders == null || folders.Length == 0)
                throw new ArgumentException("Folder không được rỗng.", nameof(folders));

            string folderPath = Path.Combine(folders);

            Directory.CreateDirectory(folderPath);

            return folderPath;
        }
    }
}
