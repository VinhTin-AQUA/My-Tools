using System.Text.Json;
using QuickTools.Core.DTOs.GoogleSheetPasswordManagerDTOs;
using QuickTools.Core.Models;
using QuickTools.Core.Responses;
using QuickTools.Services.PasswordManager;
using QuickTools.Shared.Helpers;
using QuickTools.Windows.AppSingletons;
using WebUISharp;

namespace QuickTools.Windows.Handlers.GoogleSheetPasswordManagerhandlers
{
    public static class GoogleSheetPasswordManagerHandler
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        
        public static async Task<object?> GetAll(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var config = await JsonStorageSingleton.GoogleSheetConfigInstance.GetItemAsync<GoogleSheetConfig>(GoogleSheetPasswordManagerConfigHandler.ConfigKey);
            if (string.IsNullOrWhiteSpace(config.Url) || string.IsNullOrWhiteSpace(config.Key))
            {
                return SendResponse(window, event_number,
                    CreateErrorResponse<List<GoogleSheetPasswordModel>>("GetAll", "API URL or Key is not configured."));
            }
            
            var passwords = await GoogleSheetPasswordManagerService.GetAllAsync(config.Url);
            try
            {
                passwords.ForEach(x =>
                {
                    x.AccountName = AES256Helper.Decrypt(config.Key, x.AccountName);
                    x.UserName = AES256Helper.Decrypt(config.Key, x.UserName);
                    x.Password = AES256Helper.Decrypt(config.Key, x.Password);
                    x.Note = AES256Helper.Decrypt(config.Key, x.Note);
                });

                return SendResponse(window, event_number, CreateResponse("GetAll", passwords));
            }
            catch (Exception ex)
            {
                return SendResponse(window, event_number, CreateResponse("GetAll", passwords));
            }
        }

        public static async Task<object?> Create(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var config = await JsonStorageSingleton.GoogleSheetConfigInstance.GetItemAsync<GoogleSheetConfig>(GoogleSheetPasswordManagerConfigHandler.ConfigKey);
            if (string.IsNullOrWhiteSpace(config.Url) || string.IsNullOrWhiteSpace(config.Key))
            {
                return SendResponse(window, event_number,
                    CreateErrorResponse<List<GoogleSheetPasswordModel>>("GetAll", "API URL or Key is not configured."));
            }

            var request = ReadRequest<GoogleSheetPasswordModel>(window, event_number);
            if (request == null)
            {
                return SendResponse(window, event_number,
                    CreateErrorResponse<GoogleSheetApiResult>("Create",
                        "Invalid request or API URL is not configured."));
            }

            var result = await GoogleSheetPasswordManagerService.CreateAsync(
                config.Url,
                new GoogleSheetPasswordModel
                {
                    AccountName = AES256Helper.Encrypt(config.Key, request.AccountName),
                    UserName = AES256Helper.Encrypt(config.Key, request.UserName),
                    Password = AES256Helper.Encrypt(config.Key, request.Password),
                    Note = AES256Helper.Encrypt(config.Key, request.Note),
                });

            return SendResponse(window, event_number, CreateResponse("Create", result));
        }

        public static async Task<object?> Update(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var config = await JsonStorageSingleton.GoogleSheetConfigInstance.GetItemAsync<GoogleSheetConfig>(GoogleSheetPasswordManagerConfigHandler.ConfigKey);
            var request = ReadRequest<GoogleSheetPasswordModel>(window, event_number);

            if (request == null || string.IsNullOrWhiteSpace(config.Url) || string.IsNullOrWhiteSpace(config.Key))
            {
                return SendResponse(window, event_number,
                    CreateErrorResponse<GoogleSheetApiResult>("Update",
                        "Invalid request or API URL is not configured."));
            }

            var result = await GoogleSheetPasswordManagerService.UpdateAsync(
                config.Url,
                request.Id,
                new GoogleSheetPasswordModel
                {
                    Id = request.Id,
                    AccountName = AES256Helper.Encrypt(config.Key, request.AccountName),
                    UserName = AES256Helper.Encrypt(config.Key, request.UserName),
                    Password = AES256Helper.Encrypt(config.Key, request.Password),
                    Note = AES256Helper.Encrypt(config.Key, request.Note),
                });

            return SendResponse(window, event_number, CreateResponse("Update", result));
        }

        public static async Task<object?> Delete(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var config = await JsonStorageSingleton.GoogleSheetConfigInstance.GetItemAsync<GoogleSheetConfig>(GoogleSheetPasswordManagerConfigHandler.ConfigKey);
            var request = ReadRequest<DeleteGoogleSheetPasswordManagerRequest>(window, event_number);

            if (request == null || string.IsNullOrWhiteSpace(config.Url))
            {
                return SendResponse(window, event_number,
                    CreateErrorResponse<GoogleSheetApiResult>("Delete",
                        "Invalid request or API URL is not configured."));
            }

            var result = await GoogleSheetPasswordManagerService.RemoveAsync(config.Url, request.Id);

            return SendResponse(window, event_number, CreateResponse("Delete", result));
        }
        
        public static async Task<object?> DuplicateFirstSheet(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var config = await JsonStorageSingleton.GoogleSheetConfigInstance.GetItemAsync<GoogleSheetConfig>(GoogleSheetPasswordManagerConfigHandler.ConfigKey);
            var request = ReadRequest<DeleteGoogleSheetPasswordManagerRequest>(window, event_number);

            if (request == null || string.IsNullOrWhiteSpace(config.Url) || string.IsNullOrWhiteSpace(config.Key))
            {
                return SendResponse(window, event_number,
                    CreateErrorResponse<GoogleSheetApiResult>("DuplicateFirstSheet",
                        "Invalid request or API URL is not configured."));
            }

            try
            {
                var result = await GoogleSheetPasswordManagerService.DuplicateFirstSheet(config.Url);
                return SendResponse(window, event_number, CreateResponse("DuplicateFirstSheet", result));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static T? ReadRequest<T>(UIntPtr window, UIntPtr eventNumber) where T : class
        {
            var dataPtr = WebUI.InterfaceGetStringAt(window, eventNumber, UIntPtr.Zero);
            var jsonData = WebUI.PtrToString(dataPtr);

            return string.IsNullOrWhiteSpace(jsonData)
                ? null
                : JsonSerializer.Deserialize<T>(jsonData, JsonOptions);
        }

        private static WebUIResponse<T> CreateResponse<T>(string action, T? data, bool success = true,
            string description = "")
        {
            return new WebUIResponse<T>
            {
                Action = action,
                Title = action,
                Description = description,
                Success = success,
                Data = data
            };
        }

        private static WebUIResponse<T> CreateErrorResponse<T>(string action, string description)
        {
            return CreateResponse<T>(action, default, false, description);
        }

        private static WebUIResponse<T> SendResponse<T>(UIntPtr window, UIntPtr eventNumber,
            WebUIResponse<T> response)
        {
            WebUI.InterfaceSetResponse(window, eventNumber, JsonSerializer.Serialize(response, JsonOptions));
            return response;
        }
    }
}
