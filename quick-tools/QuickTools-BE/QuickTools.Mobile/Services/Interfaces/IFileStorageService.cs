using Microsoft.AspNetCore.Components.Forms;

namespace QuickTools.Mobile.Services.Interfaces
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Lưu file vào sandbox của app
        /// </summary>
        Task<string> SaveFileAsync(IBrowserFile file, string subFolder = "images");
        
        Task<string> SaveFileAsync(byte[] fileBytes, string fileName, string subFolder = "images");
        
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string subFolder = "images");
        
        /// <summary>
        /// Đọc file từ sandbox
        /// </summary>
        Task<byte[]> LoadFileAsync(string filePath);
        
        /// <summary>
        /// Lấy file dưới dạng Base64 để preview
        /// </summary>
        Task<string> GetFileAsBase64Async(string filePath);
        
        /// <summary>
        /// Xóa file khỏi sandbox
        /// </summary>
        Task<bool> DeleteFileAsync(string filePath);
        
        /// <summary>
        /// Lấy danh sách file trong thư mục
        /// </summary>
        List<string> GetFiles(string subFolder = "images");
        
        /// <summary>
        /// Lấy kích thước thư mục
        /// </summary>
        long GetFolderSize(string subFolder = "images");
        
        /// <summary>
        /// Dọn dẹp file tạm
        /// </summary>
        Task CleanupTempFilesAsync(int daysOld = 7);
    }
}