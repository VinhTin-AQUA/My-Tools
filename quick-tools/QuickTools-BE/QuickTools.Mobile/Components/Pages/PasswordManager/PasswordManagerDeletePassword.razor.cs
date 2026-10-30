using Microsoft.AspNetCore.Components;
using QuickTools.Core.Models;
using QuickTools.Mobile.Constants;
using QuickTools.Mobile.Services.Implementations;
using QuickTools.Mobile.Services.Interfaces;
using QuickTools.Services.PasswordManager;

namespace QuickTools.Mobile.Components.Pages.PasswordManager
{
    public partial class PasswordManagerDeletePassword : ComponentBase
    {
        [Inject] protected NotificationService NotificationService { get; set; } = default!;
        [Inject] protected ISecureStorageService SecureStorageService { get; set; } = default!;
        
        [Parameter]
        public int? PasswordId { get; set; }

        [Parameter]
        public EventCallback<bool> VisibleChange { get; set; }

        [Parameter]
        public EventCallback<int> PasswordDeleted { get; set; }

        private bool _isDeleting;

        private async Task Close()
        {
            if (_isDeleting)
            {
                return;
            }

            await VisibleChange.InvokeAsync(false);
        }

        private async Task ConfirmDelete()
        {
            if (PasswordId is null || _isDeleting)
            {
                return;
            }

            var id = PasswordId.Value;
            _isDeleting = true;
            try
            {
                var googleSheetConfig =
                    await SecureStorageService.LoadOrDefaultAsync<GoogleSheetConfig>(AppConstants.GoogleSheetConfigKey);

                if (string.IsNullOrEmpty(googleSheetConfig.Key) || string.IsNullOrEmpty(googleSheetConfig.Url))
                {
                    throw new Exception("Google Sheet Config is empty.");
                }

                var r = await GoogleSheetPasswordManagerService.RemoveAsync(googleSheetConfig.Url, id);

                if (!r.Success)
                {
                    _isDeleting = false;

                    _ = ShowNotificationAsync("Remove password failed. Try again", $"Id = {id}");

                    return;
                }

                // Dừng loading
                _isDeleting = false;

                // Đóng popup ngay khi xoá thành công
                await VisibleChange.InvokeAsync(false);

                await PasswordDeleted.InvokeAsync(id);

                _ = ShowNotificationAsync("Remove password successfully", $"Id = {id}");
            }
            catch (Exception ex)
            {
                _isDeleting = false;

                _ = ShowNotificationAsync("Remove password failed", ex.Message);
            }
        }

        /// <summary>
        /// Gửi thông báo hệ thống mà không chặn luồng xử lý UI,
        /// tránh trường hợp popup/loading bị treo khi thông báo không hoàn tất.
        /// </summary>
        private async Task ShowNotificationAsync(string title, string message)
        {
            try
            {
                await NotificationService.ShowAsync(1, title, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PasswordManager] Notification failed: {ex.Message}");
            }
        }
    }
}
