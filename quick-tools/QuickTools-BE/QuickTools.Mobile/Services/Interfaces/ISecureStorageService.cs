namespace QuickTools.Mobile.Services.Interfaces
{
    public interface ISecureStorageService
    {
        Task<bool> SaveAsync<T>(string key, T data) where T : class, new();
        Task<T?> LoadAsync<T>(string key) where T : class, new();
        Task<T> LoadOrDefaultAsync<T>(string key) where T : class, new();
        Task<bool> ExistsAsync(string key);
        Task<bool> DeleteAsync(string key);
        Task<bool> UpdateAsync<T>(string key, Action<T> updateAction) where T : class, new();
        Task<string?> GetRawJsonAsync(string key);
        Task<bool> SaveListAsync<T>(string key, List<T> data) where T : class, new();
        Task<List<T>> LoadListAsync<T>(string key) where T : class, new();
    }
}