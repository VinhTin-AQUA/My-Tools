using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using QuickTools.Mobile.Services.Interfaces;

namespace QuickTools.Mobile.Services.Implementations
{
public class FileStorageService : IFileStorageService
    {
        private readonly string _baseDirectory;
        private readonly ILogger<FileStorageService> _logger;
        private readonly long _maxFileSize = 20 * 1024 * 1024; // 20MB

        public FileStorageService(ILogger<FileStorageService> logger)
        {
            _logger = logger;
            _baseDirectory = FileSystem.AppDataDirectory;
            InitializeDirectories();
        }

        private void InitializeDirectories()
        {
            try
            {
                var subDirs = new[] { "images", "temp", "documents", "thumbnails" };
                foreach (var dir in subDirs)
                {
                    var path = Path.Combine(_baseDirectory, dir);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        _logger.LogInformation($"Created directory: {path}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize directories");
                throw;
            }
        }

        public async Task<string> SaveFileAsync(IBrowserFile file, string subFolder = "images")
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            try
            {
                var folderPath = Path.Combine(_baseDirectory, subFolder);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // Tạo tên file an toàn
                var safeFileName = $"{Guid.NewGuid()}_{SanitizeFileName(file.Name)}";
                var filePath = Path.Combine(folderPath, safeFileName);

                // Kiểm tra dung lượng
                if (file.Size > _maxFileSize)
                    throw new InvalidOperationException($"File {file.Name} exceeds maximum size of {_maxFileSize / 1024 / 1024}MB");

                // Lưu file
                using var stream = file.OpenReadStream(maxAllowedSize: _maxFileSize);
                using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                await stream.CopyToAsync(fileStream);

                _logger.LogInformation($"File saved successfully: {filePath} (Size: {file.Size} bytes)");
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to save file {file.Name}");
                throw;
            }
        }

        public async Task<string> SaveFileAsync(byte[] fileBytes, string fileName, string subFolder = "images")
        {
            if (fileBytes == null || fileBytes.Length == 0)
                throw new ArgumentNullException(nameof(fileBytes));
            
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            try
            {
                var folderPath = Path.Combine(_baseDirectory, subFolder);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var safeFileName = $"{Guid.NewGuid()}_{SanitizeFileName(fileName)}";
                var filePath = Path.Combine(folderPath, safeFileName);

                await File.WriteAllBytesAsync(filePath, fileBytes);

                _logger.LogInformation($"File saved from bytes: {filePath} (Size: {fileBytes.Length} bytes)");
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to save file from bytes: {fileName}");
                throw;
            }
        }
        
        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string subFolder = "images")
        {
            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));
            
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            try
            {
                var folderPath = Path.Combine(_baseDirectory, subFolder);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var safeFileName = $"{Guid.NewGuid()}_{SanitizeFileName(fileName)}";
                var filePath = Path.Combine(folderPath, safeFileName);

                using var fileStreamOutput = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                await fileStream.CopyToAsync(fileStreamOutput);

                _logger.LogInformation($"File saved from stream: {filePath}");
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to save file from stream: {fileName}");
                throw;
            }
        }
        
        public async Task<byte[]> LoadFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            try
            {
                return await File.ReadAllBytesAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to load file: {filePath}");
                throw;
            }
        }

        public async Task<string> GetFileAsBase64Async(string filePath)
        {
            var bytes = await LoadFileAsync(filePath);
            var contentType = GetContentType(filePath);
            return $"data:{contentType};base64,{Convert.ToBase64String(bytes)}";
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation($"File deleted: {filePath}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete file: {filePath}");
                return false;
            }
        }

        public List<string> GetFiles(string subFolder = "images")
        {
            var folderPath = Path.Combine(_baseDirectory, subFolder);
            if (!Directory.Exists(folderPath))
                return new List<string>();

            return Directory.GetFiles(folderPath).ToList();
        }

        public long GetFolderSize(string subFolder = "images")
        {
            var folderPath = Path.Combine(_baseDirectory, subFolder);
            if (!Directory.Exists(folderPath))
                return 0;

            try
            {
                return Directory.GetFiles(folderPath)
                               .Sum(file => new FileInfo(file).Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get folder size: {folderPath}");
                return 0;
            }
        }

        public async Task CleanupTempFilesAsync(int daysOld = 7)
        {
            try
            {
                var tempFolder = Path.Combine(_baseDirectory, "temp");
                if (!Directory.Exists(tempFolder))
                    return;

                var cutoffDate = DateTime.Now.AddDays(-daysOld);
                var oldFiles = Directory.GetFiles(tempFolder)
                                        .Where(f => File.GetCreationTime(f) < cutoffDate)
                                        .ToList();

                foreach (var file in oldFiles)
                {
                    await DeleteFileAsync(file);
                }

                if (oldFiles.Any())
                {
                    _logger.LogInformation($"Cleaned up {oldFiles.Count} temp files older than {daysOld} days");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cleanup temp files");
            }
        }

        private string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Concat(fileName.Split(invalidChars));
        }

        private string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".svg" => "image/svg+xml",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
    }
}