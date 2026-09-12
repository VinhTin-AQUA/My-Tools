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
    public partial class AddIconComponent : ComponentBase
    {
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }
        private string Name { get; set; } = string.Empty;
        private string Link { get; set; } = string.Empty;
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
            if (isSubmitting) return;
            Name = "";
            Link = "";
            await IsVisibleChanged.InvokeAsync(false);
        }

        private async Task Submit()
        {
            if (isSubmitting) return; 
            isSubmitting = true;
            
            try
            {
                var request = new IconModel
                {
                    Name = Name,
                    Url = Link,
                    IconType = IconType,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _iconService.CreateAsync(request);
                await NotificationService.ShowAsync(
                    1,
                    "Add icon successfully",
                    $"{Name} icon added");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"");
                await NotificationService.ShowAsync(
                    1,
                    "Add icon failed",
                    $"Submit error: {ex}");
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