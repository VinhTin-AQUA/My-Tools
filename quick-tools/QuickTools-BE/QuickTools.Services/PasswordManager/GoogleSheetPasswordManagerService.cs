using System.Net.Http.Json;
using System.Text.Json;
using QuickTools.Core.Models;

namespace QuickTools.Services.PasswordManager
{
    public class GoogleSheetPasswordManagerService
    {
        private static readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        static GoogleSheetPasswordManagerService()
        {
            // _apiUrl = apiUrl.TrimEnd('/');
            _httpClient = new HttpClient();
        }

        // Get all
        public static async Task<List<GoogleSheetPasswordModel>> GetAllAsync(string apiUrl)
        {
            var url = $"{apiUrl}?action=getAll";
            
            var response = await _httpClient.GetFromJsonAsync<List<GoogleSheetPasswordModel>>(
                url, JsonOptions);
            
            return response ?? new List<GoogleSheetPasswordModel>();
        }

        // CREATE: Thêm tài khoản
        public static async Task<GoogleSheetApiResult> CreateAsync(string apiUrl, GoogleSheetPasswordModel account)
        {
            var request = new GoogleSheetApiRequest<GoogleSheetPasswordModel>
            {
                Action = "create",
                Data = account
            };

            return await PostAsync(apiUrl, request);
        }

        // UPDATE: Cập nhật tài khoản
        public static async Task<GoogleSheetApiResult> UpdateAsync(string apiUrl, int id, GoogleSheetPasswordModel account)
        {
            var request = new GoogleSheetApiRequest<GoogleSheetPasswordModel>
            {
                Action = "update",
                Id = id,
                Data = account
            };

            return await PostAsync(apiUrl, request);
        }

        // DELETE: Xóa tài khoản
        public static async Task<GoogleSheetApiResult> RemoveAsync(string apiUrl, int id)
        {
            var request = new GoogleSheetApiRequest<GoogleSheetPasswordModel>
            {
                Action = "remove",
                Id = id
            };

            return await PostAsync(apiUrl, request);
        }
        
        // DUPLICATE first Sheet
        public static async Task<GoogleSheetApiResult> DuplicateFirstSheet(string apiUrl)
        {
            var request = new GoogleSheetApiRequest<GoogleSheetPasswordModel>
            {
                Action = "duplicateFirstSheet",
            };

            return await PostAsync(apiUrl, request);
        }

        private static async Task<GoogleSheetApiResult> PostAsync<T>(string apiUrl, GoogleSheetApiRequest<T> request)
        {
            var response = await _httpClient.PostAsJsonAsync(
                apiUrl, request, JsonOptions);

            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadFromJsonAsync<GoogleSheetApiResult>(JsonOptions);

            if (result == null)
                throw new InvalidOperationException(
                    "API không trả về kết quả hợp lệ.");

            if (!result.Success)
                throw new InvalidOperationException(
                    result.Message ?? "Thao tác thất bại.");

            return result;
        }
    }
}