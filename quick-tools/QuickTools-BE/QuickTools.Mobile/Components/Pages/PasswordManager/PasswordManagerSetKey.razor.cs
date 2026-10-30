using Microsoft.AspNetCore.Components;
using QuickTools.Core.Models;
using QuickTools.Mobile.Constants;
using QuickTools.Mobile.Services.Interfaces;

namespace QuickTools.Mobile.Components.Pages.PasswordManager
{
    public partial class PasswordManagerSetKey : ComponentBase
    {
        [Inject] protected ISecureStorageService SecureStorageService { get; set; } = default!;
        
        [Parameter]
        public string? ExistingKey { get; set; }

        [Parameter]
        public EventCallback<bool> VisibleChange { get; set; }

        [Parameter]
        public EventCallback<string> KeySaved { get; set; }

        private string _oldKey = string.Empty;
        private string _newKey = string.Empty;
        private string _confirmNewKey = string.Empty;

        private bool _oldKeyTouched;
        private bool _newKeyTouched;
        private bool _confirmNewKeyTouched;

        private bool _showOldKey;
        private bool _showNewKey;
        private bool _showConfirmNewKey;
        private bool _isSaving;

        /* ========================= validation ========================= */

        private string? OldKeyError
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ExistingKey) || !_oldKeyTouched)
                {
                    return null;
                }

                if (string.IsNullOrWhiteSpace(_oldKey))
                {
                    return "Old key is required.";
                }

                return _oldKey == ExistingKey
                    ? null
                    : "Old key does not match.";
            }
        }

        private string? NewKeyError =>
            !_newKeyTouched
                ? null
                : string.IsNullOrWhiteSpace(_newKey)
                    ? "New key is required."
                    : null;

        private string? ConfirmNewKeyError
        {
            get
            {
                if (!_confirmNewKeyTouched)
                {
                    return null;
                }

                if (string.IsNullOrWhiteSpace(_confirmNewKey))
                {
                    return "Please confirm the new key.";
                }

                return _confirmNewKey == _newKey
                    ? null
                    : "New keys do not match.";
            }
        }

        private bool HasErrors =>
            OldKeyError is not null ||
            NewKeyError is not null ||
            ConfirmNewKeyError is not null;

        /* ========================= actions ========================= */

        private void ToggleOldKeyVisibility()
        {
            _showOldKey = !_showOldKey;
        }

        private void ToggleNewKeyVisibility()
        {
            _showNewKey = !_showNewKey;
        }

        private void ToggleConfirmNewKeyVisibility()
        {
            _showConfirmNewKey = !_showConfirmNewKey;
        }

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

            MarkAllTouched();

            if (HasErrors)
            {
                return;
            }

            _isSaving = true;

            try
            {
                var googleSheetConfig =
                    await SecureStorageService.LoadOrDefaultAsync<GoogleSheetConfig>(AppConstants.GoogleSheetConfigKey);
                googleSheetConfig.Key = _newKey;
                await SecureStorageService.SaveAsync(AppConstants.GoogleSheetConfigKey, googleSheetConfig);
                await KeySaved.InvokeAsync(_newKey);

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

        private void MarkAllTouched()
        {
            _oldKeyTouched = true;
            _newKeyTouched = true;
            _confirmNewKeyTouched = true;
        }
    }
}
