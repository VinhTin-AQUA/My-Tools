using System.Runtime.Versioning;
using System.Text.Json;
using Microsoft.Maui.Storage;

namespace QuickTools.Services.LocalStorages
{
    /// <summary>
    ///     Service lưu trữ dữ liệu bảo mật dạng JSON - Mỗi hàm nhận key riêng
    /// </summary>
    [SupportedOSPlatform("android21.0")]
    public class SecureStorageService
    {
        private readonly JsonSerializerOptions _jsonOptions;

        public SecureStorageService()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
        }

        /// <summary>
        ///     Lưu đối tượng với key chỉ định
        /// </summary>
        public async Task<bool> SaveAsync<T>(string key, T data) where T : class, new()
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException(nameof(key), "Key không được để trống");

                if (data == null)
                    throw new ArgumentNullException(nameof(data), "Dữ liệu không được null");

                var json = JsonSerializer.Serialize(data, _jsonOptions);
                await SecureStorage.Default.SetAsync(key, json);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SecureStorage] Lỗi khi lưu key '{key}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        ///     Đọc đối tượng với key chỉ định
        /// </summary>
        public async Task<T?> LoadAsync<T>(string key) where T : class, new()
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException(nameof(key));

                var json = await SecureStorage.Default.GetAsync(key);

                if (string.IsNullOrEmpty(json))
                    return null;

                return JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SecureStorage] Lỗi khi đọc key '{key}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        ///     Đọc đối tượng, nếu không có thì trả về default
        /// </summary>
        public async Task<T> LoadOrDefaultAsync<T>(string key) where T : class, new()
        {
            var data = await LoadAsync<T>(key);
            return data ?? new T();
        }

        /// <summary>
        ///     Kiểm tra key đã tồn tại chưa
        /// </summary>
        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    return false;

                var json = await SecureStorage.Default.GetAsync(key);
                return !string.IsNullOrEmpty(json);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Xóa dữ liệu theo key
        /// </summary>
        public async Task<bool> DeleteAsync(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    return false;

                if (await ExistsAsync(key)) SecureStorage.Default.Remove(key);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SecureStorage] Lỗi khi xóa key '{key}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        ///     Cập nhật một phần dữ liệu
        /// </summary>
        public async Task<bool> UpdateAsync<T>(string key, Action<T> updateAction) where T : class, new()
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException(nameof(key));

                var existingData = await LoadAsync<T>(key);
                if (existingData == null)
                    return false;

                updateAction(existingData);
                return await SaveAsync(key, existingData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SecureStorage] Lỗi khi cập nhật key '{key}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        ///     Lấy JSON thô để debug
        /// </summary>
        public async Task<string?> GetRawJsonAsync(string key)
        {
            try
            {
                return await SecureStorage.Default.GetAsync(key);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Lưu danh sách (tiện ích)
        /// </summary>
        public async Task<bool> SaveListAsync<T>(string key, List<T> data) where T : class, new()
        {
            return await SaveAsync(key, data);
        }

        /// <summary>
        ///     Đọc danh sách (tiện ích)
        /// </summary>
        public async Task<List<T>> LoadListAsync<T>(string key) where T : class, new()
        {
            return await LoadAsync<List<T>>(key) ?? new List<T>();
        }
    }
}