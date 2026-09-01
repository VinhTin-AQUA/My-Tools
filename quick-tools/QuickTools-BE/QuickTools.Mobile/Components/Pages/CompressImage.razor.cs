using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using QuickTools.Mobile.Constants;
using QuickTools.Mobile.Services.Implementations;
using QuickTools.Mobile.Services.Interfaces;
using QuickTools.Services.Libcaesium;

namespace QuickTools.Mobile.Components.Pages
{
    public partial class CompressImage : ComponentBase
    {
        [Inject] protected IJSRuntime JS { get; set; } = default!;
        [Inject] protected IFileStorageService FileStorageService { get; set; } = default!;
        [Inject] protected IExternalStoreService ExternalStoreService { get; set; } = default!;
        [Inject] protected NotificationService NotificationService { get; set; } = default!;
        
        protected List<UploadedCompressedImage> uploadedFiles { get; set; } = [];
        protected List<ProcessedCompressedImage> processedImages { get; set; } = [];
        private int quality = 50;

        private void OnQualityInput(ChangeEventArgs e)
        {
            if (int.TryParse(e.Value?.ToString(), out var value))
            {
                quality = Math.Clamp(value, 10, 100);
            }
        }
        
        protected bool IsUploading { get; set; }
        protected int Progress { get; set; }
        
        protected double ProgressPercentage =>
            uploadedFiles.Count == 0 ? 0 : Math.Min(100, (double)Progress / uploadedFiles.Count * 100);

        protected long TotalSize()
        {
            return uploadedFiles.Sum(x => x.Size);
        }

        protected long TotalSizeProcessed()
        {
            return processedImages.Sum(x => x.Size);
        }

        protected async Task OnFilesSelected(InputFileChangeEventArgs e)
        {
            foreach (var file in e.GetMultipleFiles())
                try
                {
                    var image = new UploadedCompressedImage
                    {
                        Id = Guid.NewGuid(), 
                        Name = file.Name, 
                        Size = file.Size, 
                        ContentType = file.ContentType,
                        ProcessingStatus = CompressedFileProcessingStatus.Pending,
                    }; /* * Demo preview. * * Trong production nên upload file lên server * hoặc lưu temporary file thay vì đọc file lớn * trực tiếp vào memory. */
                    await using var stream = file.OpenReadStream(maxAllowedSize: 20 * 1024 * 1024);
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    var bytes = memoryStream.ToArray();
                    image.PreviewUrl = $"data:{file.ContentType};base64,{Convert.ToBase64String(bytes)}";
                   
                    
                    // 2. LƯU FILE VÀO SANDBOX
                    var filePath = await FileStorageService.SaveFileAsync(file, FolderConstants.Temp);
                    image.Data = bytes;
                    image.LocalPath = filePath;
                    uploadedFiles.Add(image);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to load image {file.Name}: {ex.Message}");
                }

            await InvokeAsync(StateHasChanged);
        }
        
        protected void RemoveImage(Guid id)
        {
            var image = uploadedFiles.FirstOrDefault(x => x.Id == id);
            if (image is null) return;
            uploadedFiles.Remove(image);
            // if (SelectedImage?.Id == id) ClosePopupImagePreview();
            StateHasChanged();
        }

        protected void OnClear()
        {
            uploadedFiles.Clear();
            processedImages.Clear();
            Progress = 0;
            IsUploading = false;
            // ClosePopupImagePreview();
            StateHasChanged();
        }

        protected async Task OnSubmit()
        {
            if (uploadedFiles.Count == 0 || IsUploading) return;
            
            IsUploading = true;
            Progress = 0;
            processedImages.Clear();
            
            foreach (var image in uploadedFiles)
            {
                image.ProcessingStatus = CompressedFileProcessingStatus.Processing;
                await InvokeAsync(StateHasChanged);
                try
                {
                    var outData =  LibcaesiumService.CompressInMemory((uint)quality, image.Data);
                    if (outData == null)
                    {
                        continue;
                    }
                    await ExternalStoreService.SaveToPicturesAsync(outData, $"compressed_{image.Name}");
                    
                    image.ProcessingStatus = CompressedFileProcessingStatus.Success;
                    processedImages.Add(new ProcessedCompressedImage
                    {
                        Id = Guid.NewGuid(), 
                        Name = GetProcessedFileName(image.Name), 
                        Size = outData.Length,
                        PreviewUrl = image.PreviewUrl,
                    });
                    
                    await NotificationService.ShowAsync(
                        1,
                        "Libcaesium Compress Successed",
                        image.Name);
                }
                catch(Exception ex)
                {
                    image.ProcessingStatus = CompressedFileProcessingStatus.Failed;
                    await NotificationService.ShowAsync(
                        1,
                        "Libcaesium Compress Error",
                        ex.Message);
                }

                Progress++;
                await InvokeAsync(StateHasChanged);
            }
            
            foreach (var uploadedFile in uploadedFiles)
            {
                await FileStorageService.DeleteFileAsync(uploadedFile.LocalPath);
            }

            IsUploading = false;
            await InvokeAsync(StateHasChanged);
        }

        protected string FormatFileSize(long bytes)
        {
            if (bytes <= 0) return "0 B";
            string[] sizes = ["B", "KB", "MB", "GB", "TB"];
            var order = 0;
            double size = bytes;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }

        private static string GetProcessedFileName(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var name = Path.GetFileNameWithoutExtension(fileName);
            return $"{name}_upscaled{extension}";
        }
    }
    
    public enum CompressedFileProcessingStatus
    {
        Pending,
        Processing,
        Success,
        Failed
    }

    public class UploadedCompressedImage
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string PreviewUrl { get; set; } = string.Empty;
        public bool Selected { get; set; }
        public string LocalPath { get; set; } = string.Empty;
        public CompressedFileProcessingStatus ProcessingStatus { get; set; }
        public byte[] Data { get; set; } = [];
    }

    public class ProcessedCompressedImage
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public string PreviewUrl { get; set; } = string.Empty;
    }
}