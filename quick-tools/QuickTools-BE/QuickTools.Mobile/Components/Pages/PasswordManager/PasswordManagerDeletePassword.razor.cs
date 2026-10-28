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

                if (r.Success)
                {
                    await NotificationService.ShowAsync(1, "Remove password successfully", $"Id = {id}");
                    await PasswordDeleted.InvokeAsync(id);
                }
                else
                {
                    await NotificationService.ShowAsync(1, "Remove password failed. Try again", $"Id = {id}");
                }
            }
            catch (Exception ex)
            {
                await NotificationService.ShowAsync(1, "Remove password failed", ex.Message);
            }
            finally
            {
                _isDeleting = false;
            }

            await VisibleChange.InvokeAsync(false);
        }
    }
}
