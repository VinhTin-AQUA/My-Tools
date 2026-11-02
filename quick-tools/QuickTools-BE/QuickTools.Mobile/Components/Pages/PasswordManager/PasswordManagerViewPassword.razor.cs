using Microsoft.AspNetCore.Components;
using QuickTools.Core.Models;
using QuickTools.Mobile.Constants;
using QuickTools.Mobile.Services.Implementations;
using QuickTools.Mobile.Services.Interfaces;

namespace QuickTools.Mobile.Components.Pages.PasswordManager
{
    public partial class PasswordManagerViewPassword : ComponentBase
    {
        [Inject] protected IBiometricService BiometricService { get; set; } = default!;
        [Inject] protected ISecureStorageService SecureStorageService { get; set; } = default!;
        [Inject] protected NotificationService NotificationService { get; set; } = default!;
        
        [Parameter]
        public GoogleSheetPasswordModel? Password { get; set; }

        [Parameter]
        public EventCallback<bool> VisibleChange { get; set; }

        private bool _showPassword;
        private bool _isVerified;

        private bool _isKeyMode;
        private bool _showKeyInput;
        private bool _isVerifying;
        private string _keyInput = string.Empty;
        private string? _keyError;

        private sealed record ViewPasswordField(string Label, string Value);

        private List<ViewPasswordField> Fields
        {
            get
            {
                if (Password is null)
                {
                    return [];
                }

                return
                [
                    new("ID", Password.Id.ToString()),
                    new("Account Name", Password.AccountName),
                    new("Username", Password.UserName),
                    new("Password", Password.Password),
                    new("Note", Password.Note),
                ];
            }
        }

        private string DisplayValue(ViewPasswordField field)
        {
            if (string.IsNullOrEmpty(field.Value))
            {
                return "—";
            }

            if (field.Label == "Password" && !_showPassword)
            {
                return "••••••••";
            }

            return field.Value;
        }

        /* ========================= verification ========================= */

        private void TogglePasswordVisibility()
        {
            _showPassword = !_showPassword;
        }

        private void ChooseKeyMode()
        {
            _isKeyMode = true;
            _keyError = null;
        }

        private void ToggleKeyInputVisibility()
        {
            _showKeyInput = !_showKeyInput;
        }

        private void ClearKeyError()
        {
            _keyError = null;
        }

        private void BackToOptions()
        {
            if (_isVerifying)
            {
                return;
            }

            _isKeyMode = false;
            _keyInput = string.Empty;
            _showKeyInput = false;
            _keyError = null;
        }

        private async Task VerifyWithBiometric()
        {
            if (_isVerifying)
            {
                return;
            }

            _isVerifying = true;
            try
            {
                if (!await BiometricService.IsAvailableAsync())
                {
                    await NotificationService.ShowAsync(
                        1,
                        "Biometric not supported",
                        "This device does not support biometric authentication or it has not been set up.");
                    return;
                }

                var result = await BiometricService.AuthenticateAsync(
                    "View password",
                    "Verify your identity to view the password");

                if (!result.Success)
                {
                    await NotificationService.ShowAsync(
                        1,
                        "Biometric verification failed",
                        result.Message);
                    return;
                }

                OnVerified();
            }
            finally
            {
                _isVerifying = false;
            }
        }

        private async Task VerifyWithKey()
        {
            if (_isVerifying)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_keyInput))
            {
                _keyError = "Key is required.";
                return;
            }

            _isVerifying = true;
            try
            {
                var googleSheetConfig =
                    await SecureStorageService.LoadOrDefaultAsync<GoogleSheetConfig>(AppConstants.GoogleSheetConfigKey);

                if (string.IsNullOrEmpty(googleSheetConfig.Key) || googleSheetConfig.Key != _keyInput)
                {
                    _keyError = "Key is not correct.";
                    return;
                }

                OnVerified();
            }
            finally
            {
                _isVerifying = false;
            }
        }

        private void OnVerified()
        {
            _isVerified = true;
            _showPassword = true;
            _isKeyMode = false;
            _showKeyInput = false;
            _keyInput = string.Empty;
            _keyError = null;
        }

        private async Task Close()
        {
            await VisibleChange.InvokeAsync(false);
        }

        private void CopyValue(ViewPasswordField field)
        {
            Console.WriteLine($"[PasswordManager] Copy {field.Label}: {field.Value}");
        }
    }
}
