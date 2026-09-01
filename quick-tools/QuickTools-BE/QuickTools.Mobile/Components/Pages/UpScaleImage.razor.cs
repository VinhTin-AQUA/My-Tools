using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using QuickTools.Mobile.Services.Implementations;
using QuickTools.Mobile.Services.Interfaces;
using QuickTools.Services.Iloveimg;
using QuickTools.Services.Models.Iloveimg;

namespace QuickTools.Mobile.Components.Pages
{
    public partial class UpScaleImage : ComponentBase
    {
        [Inject] protected IJSRuntime JS { get; set; } = default!;
        [Inject] protected IFileStorageService FileStorageService { get; set; } = default!;
        [Inject] protected IExternalStoreService ExternalStoreService { get; set; } = default!;
        [Inject] protected NotificationService NotificationService { get; set; } = default!;
        
        protected List<UploadedImage> _uploadedFiles { get; set; } = [];
        protected List<ProcessedImage> _processedImages { get; set; } = [];
        protected List<int> _sizeMultiplierOptions { get; } = [1, 2, 4];
        protected int _scale { get; set; } = 2;
        protected bool _isUploading { get; set; }
        protected int _progress { get; set; }

        private List<IloveImgUpscaleImageRequestItem> _upscaleImageRequestItems { get; set; } = [];

        protected double ProgressPercentage =>
            _uploadedFiles.Count == 0 ? 0 : Math.Min(100, (double)_progress / _uploadedFiles.Count * 100);
        
        private bool _hasPermission { get; set; } = false;
        
        protected override async Task OnInitializedAsync()
        {
           _hasPermission = await ExternalStoreService.CheckStoragePermissionAsync();
        
            if (!_hasPermission)
            {
                _hasPermission = await ExternalStoreService.RequestStoragePermissionAsync();

                if (!_hasPermission)
                {
                    // quay lại màng hình trước đó
                    await JS.InvokeVoidAsync("history.back");
                    // Navigation.NavigateTo("/");
                    // Navigation.NavigateTo("../");
                }
            }
        
            StateHasChanged();
        }
        
        protected long TotalSize()
        {
            return _uploadedFiles.Sum(x => x.Size);
        }

        protected long TotalSizeProcessed()
        {
            return _processedImages.Sum(x => x.Size);
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
                    _uploadedFiles.Add(image);
                    
                    // 2. LƯU FILE VÀO SANDBOX
                    var filePath = await FileStorageService.SaveFileAsync(file, "images");
                    
                    _upscaleImageRequestItems.Add(new()
                    {
                        Base64 = "",
                        Id = image.Id.ToString(),
                        Name = image.Name,
                        LocalPath = filePath,
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to load image {file.Name}: {ex.Message}");
                }

            await InvokeAsync(StateHasChanged);
        }

        protected void RemoveImage(Guid id)
        {
            var image = _uploadedFiles.FirstOrDefault(x => x.Id == id);
            if (image is null) return;
            _uploadedFiles.Remove(image);
            StateHasChanged();
        }

        protected void OnClear()
        {
            _uploadedFiles.Clear();
            _processedImages.Clear();
            _progress = 0;
            _isUploading = false;
            StateHasChanged();
        }

        protected async Task OnSubmit()
        { 
            if (_uploadedFiles.Count == 0 || _isUploading) return;
            _isUploading = true;
            _progress = 0;
            _processedImages.Clear();

            (var uploadResponses, var server, var taskId) = await IloveImgUpscaleImageService.UploadServer(new()
            {
                Scale = _scale.ToString(),
                UpscaleImageRequestItems = _upscaleImageRequestItems,
            });

            foreach (var uploadResponse in uploadResponses)
            {
                try
                {
                    var bytes = await IloveImgUpscaleImageService.Upscale(uploadResponse, server, taskId,
                        _scale.ToString());
                    var fileName = $"IloveImgUpscale_{uploadResponse.server_filename}";
                    await ExternalStoreService.SaveToPicturesAsync(bytes, fileName);
                    
                    _processedImages.Add(new()
                    {
                        Id = Guid.Parse(uploadResponse.Id),
                        Name = uploadResponse.Name,
                        PreviewUrl = "",
                        Size = bytes.Length
                    });
                    
                    await NotificationService.ShowAsync(
                        1,
                        "IloveImg Upscale Successed",
                        uploadResponse.Name);
                }
                catch (Exception ex)
                {
                    await NotificationService.ShowAsync(
                        1,
                        "IloveImg Upscale Error",
                        ex.Message);
                }
            }

            foreach (var _upscaleImageRequestItem in _upscaleImageRequestItems)
            {
                await FileStorageService.DeleteFileAsync(_upscaleImageRequestItem.LocalPath);
            }

            _isUploading = false;
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