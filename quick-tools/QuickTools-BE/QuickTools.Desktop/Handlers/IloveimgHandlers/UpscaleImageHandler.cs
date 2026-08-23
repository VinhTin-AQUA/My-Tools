using System.Text.Json;
using QuickTools.Core.Constants;
using QuickTools.Core.Enums;
using QuickTools.Core.Notifications;
using QuickTools.Services.Iloveimg;
using QuickTools.Services.Models.Iloveimg;
using QuickTools.Windows.Helpers;
using WebUISharp;

namespace QuickTools.Windows.Handlers.IloveimgHandlers
{
    public static class UpscaleImageHandler
    {
        public static async Task<object?> UpscaleImage(UIntPtr window, UIntPtr event_type, IntPtr element, UIntPtr event_number, UIntPtr bind_id)
        {
            IntPtr dataPtr = WebUI.InterfaceGetStringAt(window, event_number, UIntPtr.Zero);
            string jsonData = WebUI.PtrToString(dataPtr);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true 
            };
            UpscaleImageRequest? upscaleImageRequest = JsonSerializer.Deserialize<UpscaleImageRequest>(jsonData, options);

            if (upscaleImageRequest == null)
            {
                SendNoticationToClient(window, "ILoveimg - Upscale Image", "Cannot parse upscale image.", false);
                return null;
            }
            
            for (int i = 0; i < upscaleImageRequest.UpscaleImageRequestItems.Count; i++)
            {
                string path = await FilesHelper.SaveBase64File(
                    upscaleImageRequest.UpscaleImageRequestItems[i].Base64, 
                    upscaleImageRequest.UpscaleImageRequestItems[i].Name, 
                    EFolder.Iloveimg_Upscale_Temp
                );
                upscaleImageRequest.UpscaleImageRequestItems[i].LocalPath = path;
            }
            
            (List<UploadResponse> uploadResponses, string server, string taskId) = ([], "", "");
            try
            {
                (uploadResponses, server, taskId) = await UpscaleImageService.UploadServer(upscaleImageRequest);
            }
            catch (Exception ex)
            {
                SendNoticationToClient(window, "ILoveimg - Upscale Image", ex.Message, false);
            }

            foreach (UploadResponse uploadResponse in uploadResponses)
            {
                try
                {
                    var filePath = await UpscaleImageService.Upscale(
                        uploadResponse,
                        server, taskId,
                        upscaleImageRequest.Scale,
                        FolderHelper.GetSystemPath(SystemFolder.Downloads)
                    );
                    var fileInfor = FilesHelper.GetInfo(filePath);
                    
                    SendNoticationToClient(window, "ILoveimg - Upscale Image", "Success", true, new()
                    {
                        Id = uploadResponse.Id,
                        Name = uploadResponse.Name,
                        ProcessingStatus = EProcessingStatus.SUCCESS,
                        LocalPath = Path.GetDirectoryName(filePath) ?? FolderHelper.GetSystemPath(SystemFolder.Downloads),
                        Size = fileInfor.Size
                    });
                }
                catch (Exception e)
                {
                    SendNoticationToClient(window, "ILoveimg - Upscale Image - Failed", e.Message, false);
                }
            }
            return upscaleImageRequest;
        }

        private static void SendNoticationToClient(UIntPtr window, string title, string message, bool isSuccess, UpscaleImageResponseItem? upscaleImageResponse = null)
        {
            upscaleImageResponse = upscaleImageResponse ?? new UpscaleImageResponseItem()
            {
                Id = "",
                Name = "",
                ProcessingStatus = EProcessingStatus.FAILED,
                LocalPath = "",
                Size = 0
            };
            const string scaleNotification = "scaleNotification";
            var noti = new Notification<UpscaleImageResponseItem>
            {
                Action = ActionConstants.ILoveimg_ScaleImg,
                IsSuccess = isSuccess,
                Message = message,
                Title = title,
                Data = upscaleImageResponse
            };
            WebUI.SendRaw(window, scaleNotification, noti);
        }
    }
}