using Android.Content;
using Android.OS;
using Android.Provider;
using Microsoft.Extensions.Logging;
using QuickTools.Mobile.Constants;
using QuickTools.Mobile.Services.Interfaces;
using Application = Android.App.Application;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;

namespace QuickTools.Mobile.Services.Implementations
{
    public class ExternalStoreService : IExternalStoreService
    {
        private readonly string _baseDirectory;
        private readonly ILogger<FileStorageService> _logger;

        public ExternalStoreService(ILogger<FileStorageService> logger)
        {
            _logger = logger;
            _baseDirectory = FileSystem.AppDataDirectory;
            InitializeDirectories();
        }

        private void InitializeDirectories()
        {
            var subDirs = new[] { FolderConstants.Images, FolderConstants.Temp, FolderConstants.Thumbnails };
            foreach (var dir in subDirs)
            {
                var path = Path.Combine(_baseDirectory, dir);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
        }
        
        // ===== LƯU VÀO PICTURES - MEDIASTORE =====
        public async Task<string?> SaveToPicturesAsync(byte[] fileBytes, string fileName)
        {
            try
            {
                // Kiểm tra quyền
                if (!await CheckStoragePermissionAsync())
                {
                    _logger.LogWarning("Storage permission not granted");
                    return null;
                }

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Q) // Android 10+
                    return await SaveUsingMediaStoreAsync(fileBytes, fileName);

                // Android 9-
                return await SaveLegacyAsync(fileBytes, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to save to Pictures: {fileName}");
                throw;
            }
        }


        // ✅ CÁCH 1: MediaStore - Hiện đại nhất (Android 10+)
        private async Task<string> SaveUsingMediaStoreAsync(byte[] fileBytes, string fileName)
        {
            try
            {
                var context = Application.Context;

                // Tạo ContentValues cho MediaStore
                var contentValues = new ContentValues();
                contentValues.Put(MediaStore.IMediaColumns.DisplayName, fileName);
                contentValues.Put(MediaStore.IMediaColumns.MimeType, GetMimeType(fileName));
                contentValues.Put(MediaStore.IMediaColumns.DateAdded, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                contentValues.Put(MediaStore.IMediaColumns.DateModified, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                contentValues.Put(MediaStore.IMediaColumns.Size, fileBytes.Length);

                // Relative path cho Android 10+
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                {
                    contentValues.Put(MediaStore.IMediaColumns.RelativePath,
                        $"{Environment.DirectoryPictures}/{AppConstants.AppName}");
                }
                else
                {
                    // Android 9-: lưu vào Pictures
                    var picturesDir = Environment.GetExternalStoragePublicDirectory(
                        Environment.DirectoryPictures);
                    var appDir = new Java.IO.File(picturesDir, AppConstants.AppName);
                    if (!appDir.Exists())
                        appDir.Mkdirs();

                    contentValues.Put(MediaStore.IMediaColumns.Data,
                        Path.Combine(appDir.AbsolutePath, fileName));
                }

                // Insert vào MediaStore
                var uri = context.ContentResolver.Insert(
                    MediaStore.Images.Media.ExternalContentUri,
                    contentValues);

                if (uri == null)
                    throw new Exception("Failed to create MediaStore entry");

                // Ghi file
                using (var outputStream = context.ContentResolver.OpenOutputStream(uri))
                {
                    if (outputStream == null)
                        throw new Exception("Failed to open output stream");

                    await outputStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                    await outputStream.FlushAsync();
                }

                _logger.LogInformation($"File saved to MediaStore: {uri}");

                // Thông báo cho Gallery
                NotifyGalleryUpdate(uri.ToString());

                return uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MediaStore save failed");
                throw;
            }
        }

        // ✅ CÁCH 2: Legacy (Android 9-)
        private async Task<string> SaveLegacyAsync(byte[] fileBytes, string fileName)
        {
            var picturesPath = Environment.GetExternalStoragePublicDirectory(
                Environment.DirectoryPictures).AbsolutePath;

            var appFolder = Path.Combine(picturesPath, AppConstants.AppName);
            if (!Directory.Exists(appFolder))
                Directory.CreateDirectory(appFolder);

            var filePath = Path.Combine(appFolder, fileName);
            await File.WriteAllBytesAsync(filePath, fileBytes);

            // Scan file để hiển thị trong Gallery
            ScanFile(filePath);

            _logger.LogInformation($"File saved to Pictures (legacy): {filePath}");
            return filePath;
        }

        // ✅ Helper: Scan file
        private void ScanFile(string filePath)
        {
            try
            {
                var context = Application.Context;
                var intent = new Intent(Intent.ActionMediaScannerScanFile);
                var file = new Java.IO.File(filePath);
                var uri = Uri.FromFile(file);
                intent.SetData(uri);
                context.SendBroadcast(intent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scan file failed");
            }
        }

        // ✅ Helper: Notify Gallery
        private void NotifyGalleryUpdate(string uriString)
        {
            try
            {
                var context = Application.Context;
                var intent = new Intent(Intent.ActionMediaScannerScanFile);
                var uri = Uri.Parse(uriString);
                intent.SetData(uri);
                context.SendBroadcast(intent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gallery notification failed");
            }
        }

        private string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
        
        // ===== PERMISSION HELPERS =====
        public async Task<bool> CheckStoragePermissionAsync()
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu) // Android 13+
                {
                    var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                    return status == PermissionStatus.Granted;
                }

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Q) // Android 10-12
                {
                    var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                    return status == PermissionStatus.Granted;
                }
                else // Android 9-
                {
                    var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                    return status == PermissionStatus.Granted;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RequestStoragePermissionAsync()
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu) // Android 13+
                {
                    var status = await Permissions.RequestAsync<Permissions.StorageRead>();
                    return status == PermissionStatus.Granted;
                }

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Q) // Android 10-12
                {
                    var status = await Permissions.RequestAsync<Permissions.StorageRead>();
                    return status == PermissionStatus.Granted;
                }
                else // Android 9-
                {
                    var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                    return status == PermissionStatus.Granted;
                }
            }
            catch
            {
                return false;
            }
        }

        private string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Concat(fileName.Split(invalidChars));
        }
    }
}