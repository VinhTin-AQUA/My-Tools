using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Microsoft.AspNetCore.Components;
using QuickTools.Core.DTOs.Icons;
using QuickTools.Core.Models;
using QuickTools.Mobile.Constants;
using QuickTools.Mobile.Services.Interfaces;
using QuickTools.Services.Icons;
using QuickTools.Services.MongoDB;
using Application = Android.App.Application;
using Environment = Android.OS.Environment;
using File = Java.IO.File;
using Path = System.IO.Path;

namespace QuickTools.Mobile.Components.Pages.IconMemes
{
    public partial class IconMeme : ComponentBase
    {
        private IconModel? _openedIcon;

        protected string searchTerm = "";
        protected bool showAddDialog;
        protected bool showAddMultiIconsDialog;
        protected int pageIndex = 1;
        protected int pageSize = 20;
        protected List<IconModel> icons = new();
        protected bool connected = true;
        protected bool isSearching = true;
        
        public List<string> ErrorMessages = [];

        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] protected IMongoServiceFactory MongoServiceFactory { get; set; } = default!;
        [Inject] protected ISecureStorageService SecureStorageService { get; set; } = default!;

        private IIconService _iconService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await CheckConnection();
            if (connected)
                await SearchIcons();
            isSearching = false;
        }

        protected async Task CheckConnection()
        {
            var mongoConfig = await SecureStorageService.LoadAsync<MongoDBSetting>(AppConstants.MongoConfigKey);
            if (mongoConfig == null)
            {
                connected = false;
                ErrorMessages.Add("mongoConfig is null");
                return;
            }

            try
            {
                var (context, iconService) =
                    MongoServiceFactory.CreateIconService(mongoConfig.ConnectionString, mongoConfig.DatabaseName);
                var (_connected, message) = await context.CheckConnectionAsync();

                connected = _connected;

                if (!connected) ErrorMessages.Add($"connected = false sau khi kiểm tra: {message}");

                _iconService = iconService;
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                connected = false;
                ErrorMessages.Add($"Lỗi trong quá trình lấy service và check connected: {ex.Message}");
            }

            if (!connected) return;

            connected = true;
        }

        protected async Task SearchIcons()
        {
            var iconResponse = await _iconService.SearchPaginationAsync(new SearchIconRequest
            {
                Keyword = searchTerm,
                Page = pageIndex,
                PageSize = pageSize
            });

            icons = iconResponse.Items;
        }

        protected void OnSearchInput(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? "";
        }

        protected async Task SearchFromButton()
        {
            pageIndex = 1;
            await SearchIcons();
        }

        protected async Task ClearSearch()
        {
            searchTerm = "";
            pageIndex = 1;
            await SearchIcons();
        }

        protected async Task PreviousPage()
        {
            if (pageIndex <= 1)
                return;

            pageIndex--;
            await SearchIcons();
        }

        protected async Task NextPage()
        {
            pageIndex++;
            await SearchIcons();
        }

        protected void OpenAddDialog()
        {
            showAddDialog = true;
            showAddMultiIconsDialog = false;
        }

        protected void OpenAddMultiIconDialog()
        {
            showAddMultiIconsDialog = true;
            showAddDialog = false;
        }

        protected async Task DeleteIcon(IconModel? icon)
        {
            if (icon == null)
                return;
        
            await _iconService.DeleteAsync(icon.Id);
            await SearchIcons();
        }
        
        protected void OpenIconLink(IconModel? icon)
        {
            if (icon == null)
                return;
        
            Navigation.NavigateTo(icon.Url, true);
        }

        private void OpenIconMenu(IconModel icon)
        {
            _openedIcon = icon;
        }

        private void CloseIconMenu()
        {
            _openedIcon = null;
        }
        
        private async Task SaveImageAsync(IconModel icon)
        {
            using var httpClient = new HttpClient();

            var bytes = await httpClient.GetByteArrayAsync(icon.Url);

            var fileName = GetImageFileName(icon);

            var context = Application.Context;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            {
                var values = new ContentValues();

                values.Put(
                    MediaStore.Images.Media.InterfaceConsts.DisplayName,
                    fileName);

                values.Put(
                    MediaStore.Images.Media.InterfaceConsts.MimeType,
                    GetMimeType(fileName));

                values.Put(
                    MediaStore.Images.Media.InterfaceConsts.RelativePath,
                    Environment.DirectoryPictures);

                values.Put(
                    MediaStore.Images.Media.InterfaceConsts.IsPending,
                    1);

                var resolver = context.ContentResolver;

                var uri = resolver.Insert(
                    MediaStore.Images.Media.ExternalContentUri,
                    values);

                if (uri == null)
                    throw new IOException("Unable to create image file.");

                try
                {
                    using var output = resolver.OpenOutputStream(uri);

                    if (output == null)
                        throw new IOException("Unable to open image stream.");

                    await output.WriteAsync(bytes);

                    values.Clear();
                    values.Put(
                        MediaStore.Images.Media.InterfaceConsts.IsPending,
                        0);

                    resolver.Update(uri, values, null, null);
                }
                catch
                {
                    resolver.Delete(uri, null, null);
                    throw;
                }
            }
            else
            {
                var picturesPath =
                    Environment.GetExternalStoragePublicDirectory(
                        Environment.DirectoryPictures);

                if (!picturesPath!.Exists())
                    picturesPath.Mkdirs();

                var file = new File(
                    picturesPath,
                    fileName);

                await System.IO.File.WriteAllBytesAsync(
                    file.AbsolutePath,
                    bytes);

                var values = new ContentValues();

                values.Put(
                    MediaStore.Images.Media.InterfaceConsts.Data,
                    file.AbsolutePath);

                values.Put(
                    MediaStore.Images.Media.InterfaceConsts.MimeType,
                    GetMimeType(fileName));

                context.ContentResolver.Insert(
                    MediaStore.Images.Media.ExternalContentUri,
                    values);
            }

            _openedIcon = null;
        }
        
        private static string GetImageFileName(IconModel icon)
        {
            var name = string.IsNullOrWhiteSpace(icon.Name)
                ? $"image_{DateTime.Now:yyyyMMdd_HHmmss}"
                : icon.Name.Trim();

            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');

            var extension = GetImageExtension(icon.Url);

            return $"{name}{extension}";
        }
        
        private static string GetImageExtension(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                var extension = Path.GetExtension(uri.AbsolutePath);

                if (!string.IsNullOrWhiteSpace(extension))
                    return extension.ToLowerInvariant();
            }

            return ".png";
        }
        
        private static string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName)
                .TrimStart('.')
                .ToLowerInvariant();

            return extension switch
            {
                "jpg" or "jpeg" => "image/jpeg",
                "webp" => "image/webp",
                "gif" => "image/gif",
                "bmp" => "image/bmp",
                _ => "image/png"
            };
        }
    }
}