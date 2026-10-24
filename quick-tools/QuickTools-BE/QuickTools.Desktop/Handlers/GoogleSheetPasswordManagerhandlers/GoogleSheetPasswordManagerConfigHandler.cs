using System.Text.Json;
using QuickTools.Core.DTOs.GoogleSheetPasswordManagerDTOs;
using QuickTools.Core.Models;
using QuickTools.Windows.AppSingletons;
using WebUISharp;

namespace QuickTools.Windows.Handlers.GoogleSheetPasswordManagerhandlers
{
    public static class GoogleSheetPasswordManagerConfigHandler
    {
        private const string ConfigKey = "GoogleSheetConfig";

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
                return false;
            }

            var config = await GetConfigAsync();
            config.Key = request.Key;

            var saved = await SaveConfigAsync(config);
            SetKeyGoogleSheetPasswordManagerResponse setKeyGoogleSheetPasswordManagerResponse = new()
            {
                Key = request.Key,
            };
            WriteResponse(window, event_number, setKeyGoogleSheetPasswordManagerResponse);

            return setKeyGoogleSheetPasswordManagerResponse;
        }

        public static async Task<object?> VerifyKey(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var request = ReadRequest<VerifyKeyGoogleSheetPasswordManagerRequest>(window, event_number);

            if (request == null)
            {
                WriteResponse(window, event_number, false);
                return false;
            }

            var config = await GetConfigAsync();
            var isValid = !string.IsNullOrEmpty(config.Key) && config.Key == request.Key;

            VerifyKeyGoogleSheetPasswordManagerResponse verifyKeyGoogleSheetPasswordManagerResponse = new()
            {
                IsValid = isValid
            };

            WriteResponse(window, event_number, verifyKeyGoogleSheetPasswordManagerResponse);
            return verifyKeyGoogleSheetPasswordManagerResponse;
        }

        public static async Task<object?> SetUrl(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var request = ReadRequest<SetUrlGoogleSheetPasswordManagerRequest>(window, event_number);

            if (request == null)
            {
                return false;
            }

            var config = await GetConfigAsync();
            config.Url = request.Url;

            var saved = await SaveConfigAsync(config);

            SetUrlGoogleSheetPasswordManagerResponse setUrlGoogleSheetPasswordManagerResponse = new()
            {
                Url = request.Url,
            };
            WriteResponse(window, event_number, saved);

            return saved;
        }

        public static async Task<object?> GetConfig(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var config = await GetConfigAsync();

            WriteResponse(window, event_number, config);
            return config;
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

        private static void WriteResponse<T>(UIntPtr window, UIntPtr eventNumber, T data)
        {
            WebUI.InterfaceSetResponse(window, eventNumber, JsonSerializer.Serialize(data, JsonOptions));
        }
    }
}
