namespace QuickTools.Mobile.Services.Interfaces
{
    public interface IExternalStoreService
    {
        Task<string?> SaveToPicturesAsync(byte[] fileBytes, string fileName);
        Task<bool> CheckStoragePermissionAsync();
        Task<bool> RequestStoragePermissionAsync();
    }
}