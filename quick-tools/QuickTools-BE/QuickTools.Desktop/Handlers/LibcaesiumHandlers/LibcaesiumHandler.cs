using System.Text.Json;
using QuickTools.Core.Constants;
using QuickTools.Core.Enums;
using QuickTools.Core.Notifications;
using QuickTools.Services.Libcaesium;
using QuickTools.Services.Models.Libcaesium;
using QuickTools.Windows.Helpers;
using WebUISharp;

namespace QuickTools.Windows.Handlers.LibcaesiumHandlers
{
    public static class LibcaesiumHandler
    {
        public static async Task<object?> CompressImage(UIntPtr window, UIntPtr event_type, IntPtr element, UIntPtr event_number, UIntPtr bind_id)
        {
            IntPtr dataPtr = WebUI.InterfaceGetStringAt(window, event_number, UIntPtr.Zero);

            try
            {
                var jsonData = WebUI.PtrToString(dataPtr);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                };

                var compressImageRequest =
                    JsonSerializer.Deserialize<LibcaesiumCompressImageRequest>(jsonData, options);

                if (compressImageRequest == null || compressImageRequest.CompressImageRequestItems.Count == 0)
                {
                    SendNoticationToClient(window, "ILoveimg - Upscale Image", "Cannot parse upscale image.", false);
                    return null;
                }

                for (var i = 0; i < compressImageRequest.CompressImageRequestItems.Count; i++)
                {
                    var path = await FilesHelper.SaveBase64File(
                        compressImageRequest.CompressImageRequestItems[i].Base64,
                        compressImageRequest.CompressImageRequestItems[i].Name,
                        EFolder.Libcaesium_Compress_Temp
                    );
                    compressImageRequest.CompressImageRequestItems[i].LocalPath = path;
                }

                for (var i = 0; i < compressImageRequest.CompressImageRequestItems.Count; i++)
                    try
                    {
                        var filePath = LibcaesiumService.CompressImages(
                            compressImageRequest.Quality,
                            compressImageRequest.CompressImageRequestItems[i].LocalPath,
                            FolderHelper.GetSystemPath(SystemFolder.Downloads)
                        );
                        var fileInfor = FilesHelper.GetInfo(filePath);

                        SendNoticationToClient(window, "Libcaesium - Compress Image", "Success", true,
                            new LibcaesiumCompressImageResponseItem
                            {
                                Id = compressImageRequest.CompressImageRequestItems[i].Id,
                                Name = compressImageRequest.CompressImageRequestItems[i].Name,
                                ProcessingStatus = EProcessingStatus.SUCCESS,
                                LocalPath = Path.GetDirectoryName(filePath) ??
                                            FolderHelper.GetSystemPath(SystemFolder.Downloads),
                                Size = fileInfor.Size
                            });
                    }
                    catch (Exception e)
                    {
                        SendNoticationToClient(window, "ILoveimg - Upscale Image - Failed", e.Message, false);
                    }

                return compressImageRequest;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Error: {ex.Message}");
                Console.WriteLine($"Path: {ex.Path}");
                Console.WriteLine($"Line: {ex.LineNumber}, Position: {ex.BytePositionInLine}");
            }

            return null;
        }

        private static void SendNoticationToClient(UIntPtr window, string title, string message, bool isSuccess, LibcaesiumCompressImageResponseItem? compressImageResponse = null)
        {
            compressImageResponse = compressImageResponse ?? new LibcaesiumCompressImageResponseItem()
            {
                Id = "",
                Name = "",
                ProcessingStatus = EProcessingStatus.FAILED,
                LocalPath = "",
                Size = 0
            };
            const string compressNotification = "compressNotification";
            var noti = new Notification<LibcaesiumCompressImageResponseItem>
            {
                Action = ActionConstants.Libcaesium_CompressImg,
                IsSuccess = isSuccess,
                Message = message,
                Title = title,
                Data = compressImageResponse
            };
            WebUI.SendRaw(window, compressNotification, noti);
        }
    }
}