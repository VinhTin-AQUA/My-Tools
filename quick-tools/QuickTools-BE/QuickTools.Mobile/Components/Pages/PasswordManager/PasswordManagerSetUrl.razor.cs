using Microsoft.AspNetCore.Components;
using QuickTools.Core.Models;
using QuickTools.Mobile.Constants;
using QuickTools.Mobile.Services.Interfaces;

namespace QuickTools.Mobile.Components.Pages.PasswordManager
{
    public partial class PasswordManagerSetUrl : ComponentBase
    {
        [Inject] protected ISecureStorageService SecureStorageService { get; set; } = default!;
        
        [Parameter]
        public string? CurrentUrl { get; set; }

        [Parameter]
        public EventCallback<bool> VisibleChange { get; set; }

        [Parameter]
        public EventCallback<string> UrlSaved { get; set; }

        private string _url = string.Empty;
        private bool _urlTouched;
        private bool _isSaving;

        protected override void OnInitialized()
        {
            _url = CurrentUrl ?? string.Empty;
        }

        private string? UrlError =>
            !_urlTouched
                ? null
                : string.IsNullOrWhiteSpace(_url)
                    ? "URL is required."
                    : null;

        private async Task Close()
        {
            if (_isSaving)
            {
                return;
            }

            await VisibleChange.InvokeAsync(false);
        }

        private async Task OnSubmit()
        {
            if (_isSaving)
            {
                return;
            }

            _urlTouched = true;

            if (UrlError is not null)
            {
                return;
            }

            _isSaving = true;

            try
            {
                var url = _url.Trim();
                var googleSheetConfig =
                    await SecureStorageService.LoadOrDefaultAsync<GoogleSheetConfig>(AppConstants.GoogleSheetConfigKey);
                googleSheetConfig.Url = url;
                await SecureStorageService.SaveAsync(AppConstants.GoogleSheetConfigKey, googleSheetConfig);
                await UrlSaved.InvokeAsync(url);

                // Dừng loading
                _isSaving = false;

                // Đóng popup ngay khi lưu thành công
                await VisibleChange.InvokeAsync(false);
            }
            finally
            {
                _isSaving = false;
            }
        }
    }
}
