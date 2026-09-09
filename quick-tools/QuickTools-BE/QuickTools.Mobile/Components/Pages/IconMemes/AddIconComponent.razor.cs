using System.Text.Json;
using Android.Graphics.Drawables;
using Microsoft.AspNetCore.Components;
using QuickTools.Core.Enums;
using QuickTools.Core.Models;
using QuickTools.Services.Icons;

namespace QuickTools.Mobile.Components.Pages.IconMemes
{
    public partial class AddIconComponent : ComponentBase
    {
        [Parameter] public EventCallback OnClose { get; set; }

        protected string name = "";
        protected string url = "";
        protected EIconType iconType = EIconType.Gift;

        protected bool nameTouched;
        protected bool urlTouched;

        protected List<EIconType> iconTypes = new()
        {
            EIconType.Gift,
            EIconType.Image
        };

        protected bool showToast;
        protected string toastTitle = "";
        protected string toastDetail = "";
        protected string toastSeverity = "";

        protected bool NameValid =>
            !string.IsNullOrWhiteSpace(name);
        
        [Inject] protected IIconService IconService { get; set; } = default!;

        protected bool UrlValid
        {
            get
            {
                var value = url.Trim();

                return Uri.TryCreate(
                           value,
                           UriKind.Absolute,
                           out var uri)
                       && (uri.Scheme == Uri.UriSchemeHttp ||
                           uri.Scheme == Uri.UriSchemeHttps);
            }
        }

        protected bool FormValid =>
            NameValid && UrlValid;

        protected void OnNameInput(ChangeEventArgs e)
        {
            name = e.Value?.ToString() ?? "";
        }

        protected void OnUrlInput(ChangeEventArgs e)
        {
            url = e.Value?.ToString() ?? "";
        }

        protected string GetIconTypeLabel(EIconType type)
        {
            return type == EIconType.Gift ? "Gift" : "Image";
        }

        protected async Task OnClosePopup()
        {
            await OnClose.InvokeAsync();
        }

        protected async Task Submit()
        {
            nameTouched = true;
            urlTouched = true;

            if (!FormValid)
                return;

            var request = new IconModel
            {
                Name = name.Trim(),
                Url = url.Trim(),
                IconType = iconType
            };

            await IconService.CreateAsync(request);

            await Task.Delay(500);

            ShowToast(
                "success",
                "Add icon successfully",
                name.Trim());

            await Task.Delay(1000);

            await OnClose.InvokeAsync();
        }

        protected void ShowToast(
            string severity,
            string title,
            string detail)
        {
            toastSeverity = severity;
            toastTitle = title;
            toastDetail = detail;
            showToast = true;

            _ = HideToastAsync();
        }

        protected async Task HideToastAsync()
        {
            await Task.Delay(3000);

            await InvokeAsync(() =>
            {
                showToast = false;
                StateHasChanged();
            });
        }

        protected void HideToast()
        {
            showToast = false;
        }
    }
}