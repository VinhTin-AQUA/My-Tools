using System.Text.Json;
using QuickTools.Core.DTOs.GoogleSheetPasswordManagerDTOs;
using QuickTools.Core.Models;
using QuickTools.Core.Responses;
using QuickTools.Windows.AppSingletons;
using WebUISharp;

namespace QuickTools.Windows.Handlers.GoogleSheetPasswordManagerhandlers
{
    public static class GoogleSheetPasswordManagerConfigHandler
    {
        public const string ConfigKey = "GoogleSheetConfig";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public static async Task<object?> SetKey(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var request = ReadRequest<SetKeyGoogleSheetPasswordManagerRequest>(window, event_number);

            if (request == null)
            {
                return SendResponse(window, event_number,
                    CreateErrorResponse<SetKeyGoogleSheetPasswordManagerResponse>("SetKey", "Invalid request."));
            }

            var config = await GetConfigAsync();
            config.Key = request.Key;

            var saved = await SaveConfigAsync(config);
            var response = new SetKeyGoogleSheetPasswordManagerResponse
            {
                Key = request.Key,
            };

            return SendResponse(window, event_number,
                CreateResponse("SetKey", response, saved, saved ? string.Empty : "Cannot save key."));
        }

        public static async Task<object?> VerifyKey(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var request = ReadRequest<VerifyKeyGoogleSheetPasswordManagerRequest>(window, event_number);

            if (request == null)
            {
                return SendResponse(window, event_number,
                    CreateErrorResponse<VerifyKeyGoogleSheetPasswordManagerResponse>("VerifyKey", "Invalid request."));
            }

            var config = await GetConfigAsync();
            var isValid = !string.IsNullOrEmpty(config.Key) && config.Key == request.Key;

            var response = new VerifyKeyGoogleSheetPasswordManagerResponse
            {
                IsValid = isValid
            };

            return SendResponse(window, event_number, CreateResponse("VerifyKey", response));
        }

        public static async Task<object?> SetUrl(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var request = ReadRequest<SetUrlGoogleSheetPasswordManagerRequest>(window, event_number);

            if (request == null)
            {
                return SendResponse(window, event_number,
                    CreateErrorResponse<SetUrlGoogleSheetPasswordManagerResponse>("SetUrl", "Invalid request."));
            }

            var config = await GetConfigAsync();
            config.Url = request.Url;

            var saved = await SaveConfigAsync(config);
            var response = new SetUrlGoogleSheetPasswordManagerResponse
            {
                Url = request.Url,
            };

            return SendResponse(window, event_number,
                CreateResponse("SetUrl", response, saved, saved ? string.Empty : "Cannot save URL."));
        }

        
        public static async Task<object?> GetConfig(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var config = await GetConfigAsync();

            return SendResponse(window, event_number, CreateResponse("GetConfig", config));
        }

        private static async Task<GoogleSheetConfig> GetConfigAsync()
        {
            return await JsonStorageSingleton.GoogleSheetConfigInstance
                .GetItemAsync<GoogleSheetConfig>(ConfigKey) ?? new GoogleSheetConfig();
        }

        private static async Task<bool> SaveConfigAsync(GoogleSheetConfig config)
        {
            return await JsonStorageSingleton.GoogleSheetConfigInstance
                .ReplaceItemAsync(ConfigKey, config, true);
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
