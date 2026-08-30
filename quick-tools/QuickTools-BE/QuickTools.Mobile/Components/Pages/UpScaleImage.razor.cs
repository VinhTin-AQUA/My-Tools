using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace QuickTools.Mobile.Components.Pages
{
    public partial class UpScaleImage : ComponentBase
    {
        [Inject] protected IJSRuntime JS { get; set; } = default!;
        protected List<UploadedImage> uploadedFiles { get; set; } = [];
        protected List<ProcessedImage> processedImages { get; set; } = [];
        protected List<int> sizeMultiplierOptions { get; } = [1, 2, 4];
        protected int scale { get; set; } = 2;
        protected bool IsUploading { get; set; }
        protected int Progress { get; set; }
        protected bool ShowPopupImagePreview { get; set; }
        protected UploadedImage? SelectedImage { get; set; }

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
                    var image = new UploadedImage
                    {
                        Id = Guid.NewGuid(), Name = file.Name, Size = file.Size, ContentType = file.ContentType,
                        ProcessingStatus = FileProcessingStatus.Pending
                    }; /* * Demo preview. * * Trong production nên upload file lên server * hoặc lưu temporary file thay vì đọc file lớn * trực tiếp vào memory. */
                    await using var stream = file.OpenReadStream(maxAllowedSize: 20 * 1024 * 1024);
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    var bytes = memoryStream.ToArray();
                    image.PreviewUrl = $"data:{file.ContentType};base64,{Convert.ToBase64String(bytes)}";
                    uploadedFiles.Add(image);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to load image {file.Name}: {ex.Message}");
                }

            await InvokeAsync(StateHasChanged);
        }

        protected void OnImageClick(UploadedImage image)
        {
            SelectedImage = image;
            ShowPopupImagePreview = true;
        }

        protected void ClosePopupImagePreview()
        {
            ShowPopupImagePreview = false;
            SelectedImage = null;
        }

        protected void RemoveImage(Guid id)
        {
            var image = uploadedFiles.FirstOrDefault(x => x.Id == id);
            if (image is null) return;
            uploadedFiles.Remove(image);
            if (SelectedImage?.Id == id) ClosePopupImagePreview();
            StateHasChanged();
        }

        protected void OnClear()
        {
            uploadedFiles.Clear();
            processedImages.Clear();
            Progress = 0;
            IsUploading = false;
            ClosePopupImagePreview();
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
                image.ProcessingStatus = FileProcessingStatus.Processing;
                await InvokeAsync(StateHasChanged);
                try
                {
                    /* * TODO: * * Gọi API/service xử lý ảnh thực tế tại đây. * * Ví dụ: * * var result = await ImageService.UpscaleAsync( * image, * scale); */
                    await Task.Delay(500);
                    image.ProcessingStatus = FileProcessingStatus.Success;
                    processedImages.Add(new ProcessedImage
                    {
                        Id = Guid.NewGuid(), Name = GetProcessedFileName(image.Name), Size = image.Size,
                        PreviewUrl = image.PreviewUrl
                    });
                }
                catch
                {
                    image.ProcessingStatus = FileProcessingStatus.Failed;
                }

                Progress++;
                await InvokeAsync(StateHasChanged);
            }

            IsUploading = false;
            await InvokeAsync(StateHasChanged);
        }

        protected async Task OpenFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return; /* * Browser không thể tự ý mở một folder local * vì security restriction. * * Nếu path là URL: */
            if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                await JS.InvokeVoidAsync("open", path,
                    "_blank"); /* * Nếu đây là đường dẫn local trên server/desktop app, * cần xử lý bằng backend hoặc .NET Hybrid/Desktop. */
        }

        protected string GetImageItemClass(UploadedImage image)
        {
            var baseClass = "image-item group flex cursor-pointer items-center gap-3 rounded-xl border p-3 transition";
            if (image.Selected) return $"{baseClass} selected";
            return baseClass;
        }

        protected string GetStatusBadgeClass(FileProcessingStatus status)
        {
            return status switch
            {
                FileProcessingStatus.Processing => "status-badge status-processing",
                FileProcessingStatus.Success => "status-badge status-success",
                FileProcessingStatus.Failed => "status-badge status-error", _ => "status-badge"
            };
        }

        protected string GetStatusText(FileProcessingStatus status)
        {
            return status switch
            {
                FileProcessingStatus.Processing => "Processing", FileProcessingStatus.Success => "Completed",
                FileProcessingStatus.Failed => "Failed", _ => "Waiting"
            };
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
    
    public enum FileProcessingStatus
    {
        Pending,
        Processing,
        Success,
        Failed
    }

    public class UploadedImage
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string PreviewUrl { get; set; } = string.Empty;
        public bool Selected { get; set; }
        public FileProcessingStatus ProcessingStatus { get; set; }
    }

    public class ProcessedImage
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public string PreviewUrl { get; set; } = string.Empty;
    }
}