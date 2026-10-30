using Microsoft.AspNetCore.Components;
using QuickTools.Core.Models;
using QuickTools.Mobile.Constants;
using QuickTools.Mobile.Services.Implementations;
using QuickTools.Mobile.Services.Interfaces;
using QuickTools.Services.PasswordManager;
using QuickTools.Shared.Helpers;

namespace QuickTools.Mobile.Components.Pages.PasswordManager
{
    public partial class PasswordManagerEditPassword : ComponentBase
    {
        [Inject] protected NotificationService NotificationService { get; set; } = default!;
        [Inject] protected ISecureStorageService SecureStorageService { get; set; } = default!;
        
        [Parameter]
        public GoogleSheetPasswordModel? Password { get; set; }

        [Parameter]
        public EventCallback<bool> VisibleChange { get; set; }

        [Parameter]
        public EventCallback<GoogleSheetPasswordModel> PasswordUpdated { get; set; }

        private string _accountName = string.Empty;
        private string _userName = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _note = string.Empty;

        private bool _accountNameTouched;
        private bool _userNameTouched;
        private bool _passwordTouched;
        private bool _confirmPasswordTouched;

        private bool _showPassword;
        private bool _showConfirmPassword;
        private bool _isSaving;

        protected override void OnInitialized()
        {
            if (Password is null)
            {
                return;
            }

            _accountName = Password.AccountName;
            _userName = Password.UserName;
            _password = Password.Password;
            _confirmPassword = Password.Password;
            _note = Password.Note;
        }

        /* ========================= validation ========================= */

        private string? AccountNameError =>
            !_accountNameTouched
                ? null
                : string.IsNullOrWhiteSpace(_accountName)
                    ? "Account name is required."
                    : null;

        private string? UserNameError =>
            !_userNameTouched
                ? null
                : string.IsNullOrWhiteSpace(_userName)
                    ? "Username is required."
                    : null;

        private string? PasswordError =>
            !_passwordTouched
                ? null
                : string.IsNullOrWhiteSpace(_password)
                    ? "Password is required."
                    : null;

        private string? ConfirmPasswordError
        {
            get
            {
                if (!_confirmPasswordTouched)
                {
                    return null;
                }

                if (string.IsNullOrWhiteSpace(_confirmPassword))
                {
                    return "Please confirm the password.";
                }

                return _confirmPassword == _password
                    ? null
                    : "Passwords do not match.";
            }
        }

        private bool HasErrors =>
            AccountNameError is not null ||
            UserNameError is not null ||
            PasswordError is not null ||
            ConfirmPasswordError is not null;

        /* ========================= actions ========================= */

        private void TogglePasswordVisibility()
        {
            _showPassword = !_showPassword;
        }

        private void ToggleConfirmPasswordVisibility()
        {
            _showConfirmPassword = !_showConfirmPassword;
        }

        private async Task Close()
        {
            // if (_isSaving)
            // {
            //     return;
            // }

            await VisibleChange.InvokeAsync(false);
        }

        private async Task OnSubmit()
        {
            // if (_isSaving || Password is null)
            if (Password is null)
            {
                return;
            }

            MarkAllTouched();

            if (HasErrors)
            {
                return;
            }

            // _isSaving = true;

            try
            {
                var googleSheetConfig =
                    await SecureStorageService.LoadOrDefaultAsync<GoogleSheetConfig>(AppConstants.GoogleSheetConfigKey);

                if (string.IsNullOrEmpty(googleSheetConfig.Key) || string.IsNullOrEmpty(googleSheetConfig.Url))
                {
                    throw new Exception("Google Sheet Config is empty.");
                }

                var password = new GoogleSheetPasswordModel
                {
                    Id = Password.Id,
                    AccountName = AES256Helper.Encrypt(googleSheetConfig.Key, _accountName.Trim()),
                    UserName = AES256Helper.Encrypt(googleSheetConfig.Key, _userName.Trim()),
                    Password = AES256Helper.Encrypt(googleSheetConfig.Key, _password),
                    Note = AES256Helper.Encrypt(googleSheetConfig.Key, _note.Trim()),
                };

                var r = await GoogleSheetPasswordManagerService.UpdateAsync(googleSheetConfig.Url, Password.Id, password);
                if (r.Success)
                {
                    // Dừng loading
                    // _isSaving = false;

                    // Đóng popup ngay khi cập nhật thành công
                    await VisibleChange.InvokeAsync(false);

                    await PasswordUpdated.InvokeAsync(password);

                    _ = ShowNotificationAsync("Update password successfully", password.AccountName);
                }
                else
                {
                    // _isSaving = false;
                    _ = ShowNotificationAsync("Update password failed", password.AccountName);
                }
            }
            catch (Exception ex)
            {
                // _isSaving = false;
                _ = ShowNotificationAsync("Update password failed", ex.Message);
            }
        }

        private void MarkAllTouched()
        {
            _accountNameTouched = true;
            _userNameTouched = true;
            _passwordTouched = true;
            _confirmPasswordTouched = true;
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
