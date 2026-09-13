using Microsoft.AspNetCore.Components;
using QuickTools.Core.Enums;
using QuickTools.Core.Models;
using QuickTools.Mobile.Constants;
using QuickTools.Mobile.Services.Implementations;
using QuickTools.Mobile.Services.Interfaces;
using QuickTools.Services.Icons;
using QuickTools.Services.MongoDB;

namespace QuickTools.Mobile.Components.Pages.IconMemes
{
    public partial class AddMultiIconsComponent : ComponentBase
    {
        [Parameter]
        public bool IsVisible { get; set; }

        [Parameter]
        public EventCallback<bool> IsVisibleChanged { get; set; }
        
        private string Content { get; set; } = string.Empty;
        private EIconType IconType { get; set; } = EIconType.Gift;
        private bool isSubmitting;
        
        [Inject] protected NotificationService NotificationService { get; set; } = default!;
        [Inject] protected IMongoServiceFactory MongoServiceFactory { get; set; } = default!;
        [Inject] protected ISecureStorageService SecureStorageService { get; set; } = default!;
        
        private IIconService _iconService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await CheckConnection();
        }

        protected async Task CheckConnection()
        {
            var mongoConfig = await SecureStorageService.LoadAsync<MongoDBSetting>(AppConstants.MongoConfigKey);
            if (mongoConfig == null)
            {
                return;
            }

            try
            {
                var (context, iconService) =
                    MongoServiceFactory.CreateIconService(mongoConfig.ConnectionString, mongoConfig.DatabaseName);
                var (_connected, message) = await context.CheckConnectionAsync();
                
                _iconService = iconService;
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
            }
        }
        
        private async Task Close()
        {
            if (isSubmitting)
                return;
            Content = "";
            await IsVisibleChanged.InvokeAsync(false);
        }
        private async Task Submit()
        {
            if (isSubmitting)
                return;

            isSubmitting = true;
            
            try
            {
                var lines = Content
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

                    items.Add(new IconModel
                    {
                        Name = itemName,
                        Url = itemUrl,
                        IconType = IconType,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                if (items.Count == 0)
                {
                    await NotificationService.ShowAsync(
                        1,
                        "No valid items found",
                        "Please provide name and URL pairs.");
                    return;
                }

                await _iconService.CreateManyIfNotExistsAsync(items);
                await NotificationService.ShowAsync(
                    1,
                    "Add icons successfully",
                    $"{items.Count} icons added");
                Content = "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Submit error: {ex}");
            }
            finally
            {
                isSubmitting = false;
            }
        }

        private static string GetIconDescription(EIconType iconType)
        {
            return iconType switch
            {
                EIconType.Gift => "Use a gift icon", 
                EIconType.Image => "Use an image icon", 
                _ => string.Empty
            };
        }
    }
}