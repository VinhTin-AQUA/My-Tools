using System.Text.Json;
using QuickTools.Core.DTOs.GoogleSheetPasswordManagerDTOs;
using QuickTools.Core.Models;
using QuickTools.Core.Responses;
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
            if (string.IsNullOrWhiteSpace(apiUrl))
            {
                return SendResponse(window, event_number,
                    CreateErrorResponse<List<GoogleSheetPassword>>("GetAll", "API URL is not configured."));
            }

            var passwords = await GoogleSheetPasswordManagerSingleton.Instance.GetAllAsync(apiUrl);

            return SendResponse(window, event_number, CreateResponse("GetAll", passwords));
        }

        public static async Task<object?> Create(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var request = ReadRequest<GoogleSheetPassword>(window, event_number);

            if (request == null || string.IsNullOrWhiteSpace(apiUrl))
            {
                return SendResponse(window, event_number,
                    CreateErrorResponse<GoogleSheetApiResult>("Create",
                        "Invalid request or API URL is not configured."));
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

            return SendResponse(window, event_number, CreateResponse("Create", result));
        }

        public static async Task<object?> Update(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var request = ReadRequest<GoogleSheetPassword>(window, event_number);

            if (request == null || string.IsNullOrWhiteSpace(apiUrl))
            {
                return SendResponse(window, event_number,
                    CreateErrorResponse<GoogleSheetApiResult>("Update",
                        "Invalid request or API URL is not configured."));
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

            return SendResponse(window, event_number, CreateResponse("Update", result));
        }

        public static async Task<object?> Delete(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var request = ReadRequest<DeleteGoogleSheetPasswordManagerRequest>(window, event_number);

            if (request == null || string.IsNullOrWhiteSpace(apiUrl))
            {
                return SendResponse(window, event_number,
                    CreateErrorResponse<GoogleSheetApiResult>("Delete",
                        "Invalid request or API URL is not configured."));
            }

            var result = await GoogleSheetPasswordManagerSingleton.Instance.RemoveAsync(apiUrl, request.Id);

            return SendResponse(window, event_number, CreateResponse("Delete", result));
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
