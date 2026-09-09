using System.Text.Json;
using Microsoft.AspNetCore.Components;

namespace QuickTools.MobileTemplate.Components.Pages.IconMemes
{
    public partial class AddIconComponent : ComponentBase
    {
        [Parameter] public EventCallback OnClose { get; set; }

        protected string name = "";
        protected string url = "";
        protected IconType iconType = IconType.Gift;

        protected bool nameTouched;
        protected bool urlTouched;

        protected List<IconType> iconTypes = new()
        {
            IconType.Gift,
            IconType.Image
        };

        protected bool showToast;
        protected string toastTitle = "";
        protected string toastDetail = "";
        protected string toastSeverity = "";

        protected bool NameValid =>
            !string.IsNullOrWhiteSpace(name);

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

        protected string GetIconTypeLabel(IconType type)
        {
            return type == IconType.Gift ? "Gift" : "Image";
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

            var request = new
            {
                name = name.Trim(),
                url = url.Trim(),
                iconType
            };

            Console.WriteLine(
                JsonSerializer.Serialize(request));

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