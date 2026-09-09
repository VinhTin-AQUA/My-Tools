using Microsoft.AspNetCore.Components;
using QuickTools.Core.Enums;
using QuickTools.Core.Models;
using QuickTools.Services.Icons;

namespace QuickTools.Mobile.Components.Pages.IconMemes
{
    public partial class AddMultiIconsComponent : ComponentBase
    {
        [Parameter] public EventCallback OnClose { get; set; }

        protected string inputText = "";

        protected bool showToast;
        protected string toastTitle = "";
        protected string toastDetail = "";
        protected string toastSeverity = "";
        
        [Inject] protected IIconService IconService { get; set; } = default!;

        protected void OnInput(ChangeEventArgs e)
        {
            inputText = e.Value?.ToString() ?? "";
        }

        protected async Task Submit()
        {
            var lines = inputText
                .Split(new[] { '\r', '\n' },
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            var items = new List<IconModel>();

            for (var i = 0; i + 1 < lines.Count; i += 2)
            {
                var itemName = lines[i];
                var itemUrl = lines[i + 1];

                if (string.IsNullOrWhiteSpace(itemName))
                    continue;

                if (!Uri.TryCreate(
                        itemUrl,
                        UriKind.Absolute,
                        out var uri))
                    continue;

                if (uri.Scheme != Uri.UriSchemeHttp &&
                    uri.Scheme != Uri.UriSchemeHttps)
                    continue;

                items.Add(new()
                {
                    Name = itemName,
                    Url = itemUrl,
                    IconType = EIconType.Gift,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            if (items.Count == 0)
            {
                ShowToast(
                    "error",
                    "No valid items found",
                    "Please provide name and URL pairs.");

                return;
            }

            await IconService.CreateManyAsync(items);

            ShowToast(
                "success",
                "Add icons successfully",
                $"{items.Count} icons added");

            inputText = "";

            await Task.Delay(1000);

            await OnClose.InvokeAsync();
        }

        protected async Task OnClosePopup()
        {
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