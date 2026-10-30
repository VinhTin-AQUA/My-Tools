using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using QuickTools.Core.Models;
using QuickTools.Mobile.Constants;
using QuickTools.Mobile.Services.Implementations;
using QuickTools.Mobile.Services.Interfaces;
using QuickTools.Services.PasswordManager;
using QuickTools.Shared.Helpers;

namespace QuickTools.Mobile.Components.Pages.PasswordManager
{
    public partial class PasswordManager : ComponentBase
    {
        [Inject]
        protected NavigationManager Navigation { get; set; } = default!;
        
        [Inject] protected NotificationService NotificationService { get; set; } = default!;
        [Inject] protected ISecureStorageService SecureStorageService { get; set; } = default!;
        
        public List<GoogleSheetPasswordModel> _passwords = [];

        private GoogleSheetPasswordModel? _selectedPassword;

        private bool _isLoading;
        private bool _isBackingUp;

        private string? _key;
        private string _url = string.Empty;

        private bool _showSetKey;
        private bool _showSetUrl;
        private bool _showViewPassword;
        private bool _showEditPassword;
        private bool _showDeletePassword;
        private bool _showAddPassword;

        private bool _showHeaderMenu;
        private bool _showItemMenu;

        private double _headerMenuX;
        private double _headerMenuY;
        private double _itemMenuX;
        private double _itemMenuY;
        
        protected override async Task OnInitializedAsync()
        {
            await OnReload();
        }

        private async Task LoadPasswords()
        {
            var googleSheetConfig =
                await SecureStorageService.LoadOrDefaultAsync<GoogleSheetConfig>(AppConstants.GoogleSheetConfigKey);
            
            _key = string.IsNullOrWhiteSpace(googleSheetConfig.Key) ? null : googleSheetConfig.Key;
            
            if (string.IsNullOrEmpty(googleSheetConfig.Key) || string.IsNullOrEmpty(googleSheetConfig.Url))
            {
                await NotificationService.ShowAsync(1, "Google Sheet Config is empty.", $"Please set key and url ({googleSheetConfig.Key})");
                return;
            }
            
            var passwords = await GoogleSheetPasswordManagerService.GetAllAsync(googleSheetConfig.Url);
            try
            {
                passwords.ForEach(p =>
                {
                    p.AccountName = AES256Helper.Decrypt(googleSheetConfig.Key, p.AccountName.Trim());
                    p.UserName = AES256Helper.Decrypt(googleSheetConfig.Key, p.UserName.Trim());
                    p.Password = AES256Helper.Decrypt(googleSheetConfig.Key, p.Password);
                    p.Note = AES256Helper.Decrypt(googleSheetConfig.Key, p.Note);
                });
                _passwords = passwords;
            }
            catch
            {
                _passwords = passwords;
                await NotificationService.ShowAsync(1, "Key is not correct.", "");
            }
        }

        /* ========================= navigation ========================= */

        private void GoBack()
        {
            Console.WriteLine("[PasswordManager] Navigate back");
            Navigation.NavigateTo("/");
        }

        /* ========================= header actions ========================= */

        private async Task OnReload()
        {
            if (_isLoading)
            {
                return;
            }
            
            _isLoading = true;
            try
            {
                await LoadPasswords();
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OpenHeaderMenu(MouseEventArgs args)
        {
            _showItemMenu = false;
            _showHeaderMenu = true;
            _headerMenuX = args.ClientX;
            _headerMenuY = args.ClientY;
        }

        private void OnSetKey()
        {
            CloseMenus();
            Console.WriteLine("[PasswordManager] Open set key dialog");
            _showSetKey = true;
        }

        private void OnSetUrl()
        {
            CloseMenus();
            Console.WriteLine("[PasswordManager] Open set URL dialog");
            _showSetUrl = true;
        }

        private async Task OnBackup()
        {
            CloseMenus();

            if (_isBackingUp)
            {
                return;
            }
            _isBackingUp = true;

            try
            {
                var googleSheetConfig =
                    await SecureStorageService.LoadOrDefaultAsync<GoogleSheetConfig>(AppConstants.GoogleSheetConfigKey);

                if (string.IsNullOrEmpty(googleSheetConfig.Key) || string.IsNullOrEmpty(googleSheetConfig.Url))
                {
                    throw new Exception("Google Sheet Config is empty.");
                }

                var r = await GoogleSheetPasswordManagerService.DuplicateFirstSheet(googleSheetConfig.Url);
                if (r.Success)
                {
                    await NotificationService.ShowAsync(1, "Backup successfully", "");
                }
                else
                {
                    await NotificationService.ShowAsync(1, "Backup password failed", "");
                }
            }
            catch (Exception ex)
            {
                await NotificationService.ShowAsync(1, "Backup password failed", ex.Message);
            }
            finally
            {
                _isBackingUp = false;
            }
        }

        /* ========================= item actions ========================= */

        private void OpenItemMenu(MouseEventArgs args, GoogleSheetPasswordModel password)
        {
            _selectedPassword = password;
            _showHeaderMenu = false;
            _showItemMenu = true;
            _itemMenuX = args.ClientX;
            _itemMenuY = args.ClientY;
        }

        private void OnViewPassword()
        {
            if (_selectedPassword is null)
            {
                return;
            }

            CloseMenus();
            Console.WriteLine($"[PasswordManager] View password '{_selectedPassword.AccountName}'");
            _showViewPassword = true;
        }

        private void OnEditPassword()
        {
            if (_selectedPassword is null)
            {
                return;
            }

            CloseMenus();
            Console.WriteLine($"[PasswordManager] Edit password '{_selectedPassword.AccountName}'");
            _showEditPassword = true;
        }

        private void OnDeletePassword()
        {
            if (_selectedPassword is null)
            {
                return;
            }

            CloseMenus();
            Console.WriteLine($"[PasswordManager] Delete password '{_selectedPassword.AccountName}'");
            _showDeletePassword = true;
        }

        private void OpenAddPassword()
        {
            CloseMenus();
            Console.WriteLine("[PasswordManager] Open add password dialog");
            _showAddPassword = true;
        }

        private void CloseMenus()
        {
            _showHeaderMenu = false;
            _showItemMenu = false;
        }

        private static string MenuStyle(double x, double y)
        {
            return string.Create(
                CultureInfo.InvariantCulture,
                $"--pm-menu-x: {x}px; --pm-menu-y: {y}px;");
        }

        /* ========================= popup visibility ========================= */

        private void OnSetKeyVisibleChange(bool visible) => _showSetKey = visible;

        private void OnSetUrlVisibleChange(bool visible) => _showSetUrl = visible;

        private void OnViewPasswordVisibleChange(bool visible) => _showViewPassword = visible;

        private void OnEditPasswordVisibleChange(bool visible) => _showEditPassword = visible;

        private void OnDeletePasswordVisibleChange(bool visible) => _showDeletePassword = visible;

        private void OnAddPasswordVisibleChange(bool visible) => _showAddPassword = visible;

        /* ========================= popup results ========================= */

        private void OnKeySaved(string key)
        {
            _key = key;
            Console.WriteLine("[PasswordManager] Key saved");
        }

        private void OnUrlSaved(string url)
        {
            _url = url;
            Console.WriteLine($"[PasswordManager] URL saved: {url}");
        }

        private void OnPasswordAdded(GoogleSheetPasswordModel password)
        {
            var nextId = _passwords.Count == 0
                ? 1
                : _passwords.Max(item => item.Id) + 1;

            password.Id = nextId;
            _passwords.Add(password);
            Console.WriteLine($"[PasswordManager] Added '{password.AccountName}' (id: {password.Id})");
        }

        private void OnPasswordUpdated(GoogleSheetPasswordModel password)
        {
            var index = _passwords.FindIndex(item => item.Id == password.Id);
            if (index >= 0)
            {
                _passwords[index] = password;
            }

            if (_selectedPassword?.Id == password.Id)
            {
                _selectedPassword = password;
            }
            Console.WriteLine($"[PasswordManager] Updated '{password.AccountName}' (id: {password.Id})");
        }

        private void OnPasswordDeleted(int id)
        {
            _passwords.RemoveAll(item => item.Id == id);
            if (_selectedPassword?.Id == id)
            {
                _selectedPassword = null;
            }
            Console.WriteLine($"[PasswordManager] Deleted password (id: {id})");
        }
    }
}
