using System.Text.Json;
using QuickTools.Core.DTOs.GoogleSheetPasswordManagerDTOs;
using QuickTools.Core.Models;
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
        
        private static string apiUrl = "";

        public static void Init()
        {
            // string apiUrl = "";
            apiUrl =
                "https://script.google.com/macros/s/AKfycbx7cEmb0KseReqVFwqYnu-tjMAxy3_A7Wgm_dPZPET2Hvq0Nj-ECgOfSpX4UMMsAhY/exec";
        }

        public static async Task<object?> GetAll(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var passwords = await GoogleSheetPasswordManagerSingleton.Instance.GetAllAsync(apiUrl);
            WriteResponse(window, event_number, passwords);

            return passwords;
        }

        public static async Task<object?> Create(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var request = ReadRequest<GoogleSheetPassword>(window, event_number);

            if (request == null || string.IsNullOrWhiteSpace(apiUrl))
            {
                return null;
            }

            var result = await GoogleSheetPasswordManagerSingleton.Instance.CreateAsync(
                apiUrl,
                new GoogleSheetPassword
                {
                    AccountName = request.AccountName,
                    UserName = request.UserName,
                    Password = request.Password,
                    Note = request.Note
                });
            WriteResponse(window, event_number, result);

            return result;
        }

        public static async Task<object?> Update(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var request = ReadRequest<GoogleSheetPassword>(window, event_number);

            if (request == null || string.IsNullOrWhiteSpace(apiUrl))
            {
                return null;
            }

            var result = await GoogleSheetPasswordManagerSingleton.Instance.UpdateAsync(
                apiUrl,
                request.Id,
                new GoogleSheetPassword
                {
                    Id = request.Id,
                    AccountName = request.AccountName,
                    UserName = request.UserName,
                    Password = request.Password,
                    Note = request.Note
                });
            WriteResponse(window, event_number, result);

            return result;
        }

        public static async Task<object?> Delete(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var request = ReadRequest<DeleteGoogleSheetPasswordManagerRequest>(window, event_number);

            if (request == null || string.IsNullOrWhiteSpace(apiUrl))
            {
                return null;
            }

            var result = await GoogleSheetPasswordManagerSingleton.Instance.RemoveAsync(apiUrl, request.Id);
            WriteResponse(window, event_number, result);

            return result;
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
