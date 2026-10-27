using System.Net.Http.Json;
using System.Text.Json;
using QuickTools.Core.Models;

namespace QuickTools.Services.PasswordManager
{
    public class GoogleSheetPasswordManagerService
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public GoogleSheetPasswordManagerService(HttpClient? httpClient = null)
        {
            // _apiUrl = apiUrl.TrimEnd('/');
            _httpClient = httpClient ?? new HttpClient();
        }

        // Get all
        public async Task<List<GoogleSheetPassword>> GetAllAsync(string apiUrl)
        {
            var url = $"{apiUrl}?action=getAll";
            
            var response = await _httpClient.GetFromJsonAsync<List<GoogleSheetPassword>>(
                url, JsonOptions);
            
            return response ?? new List<GoogleSheetPassword>();
        }

        // CREATE: Thêm tài khoản
        public async Task<GoogleSheetApiResult> CreateAsync(string apiUrl, GoogleSheetPassword account)
        {
            var request = new GoogleSheetApiRequest<GoogleSheetPassword>
            {
                Action = "create",
                Data = account
            };

            return await PostAsync(apiUrl, request);
        }

        // UPDATE: Cập nhật tài khoản
        public async Task<GoogleSheetApiResult> UpdateAsync(string apiUrl, int id, GoogleSheetPassword account)
        {
            var request = new GoogleSheetApiRequest<GoogleSheetPassword>
            {
                Action = "update",
                Id = id,
                Data = account
            };

            return await PostAsync(apiUrl, request);
        }

        // DELETE: Xóa tài khoản
        public async Task<GoogleSheetApiResult> RemoveAsync(string apiUrl, int id)
        {
            var request = new GoogleSheetApiRequest<GoogleSheetPassword>
            {
                Action = "remove",
                Id = id
            };

            return await PostAsync(apiUrl, request);
        }
        
        // DUPLICATE first Sheet
        public async Task<GoogleSheetApiResult> DuplicateFirstSheet(string apiUrl)
        {
            var request = new GoogleSheetApiRequest<GoogleSheetPassword>
            {
                Action = "duplicateFirstSheet",
            };

            return await PostAsync(apiUrl, request);
        }

        private async Task<GoogleSheetApiResult> PostAsync<T>(string apiUrl, GoogleSheetApiRequest<T> request)
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