using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using QuickTools.Core.Constants;
using QuickTools.Core.Models;
using QuickTools.Mobile.Components.Pages.Settings;
using QuickTools.Mobile.Services.Interfaces;
using QuickTools.Services.Icons;
using QuickTools.Services.MongoDB;

namespace QuickTools.Mobile.Components.Pages.IconMemes
{
    public partial class IconMeme : ComponentBase
    {
        private readonly string _mongoConfigKey = "MongoConfigKey";
        
        protected string searchTerm = "";
        protected bool showAddDialog;
        protected bool showAddMultiIconsDialog;
        protected int pageIndex = 1;
        protected int pageSize = 20;
        protected List<IconModel> icons = new();
        protected bool connected = true;
        protected IconModel? menuIcon;

        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] protected IIconService IconService { get; set; } = default!;
        [Inject] protected ISecureStorageService SecureStorageService { get; set; } = default!;
        
        protected override async Task OnInitializedAsync()
        {
            await CheckConnection();

            if (connected)
                await SearchIcons();
        }

        protected async Task CheckConnection()
        {
            var mongoConfig = await SecureStorageService.LoadAsync<MongoConfig>(_mongoConfigKey);

            if (mongoConfig == null)
            {
                connected = false;
                return;
            }
            
            var connectionString = mongoConfig.MongoConnectionString;
            var databaseName = mongoConfig.MongoDatabaseName ;

            // bool checkConnection = false;
            MongoDbContext? context = null;
            
            try
            {
                context = new MongoDbContext(connectionString, databaseName);
                connected = await context.CheckConnectionAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ MongoDB connection failed: {ex.Message}");
                connected = false;
            }
            
            if (!connected || context == null)
            {
                return;
            }
            
            connected = true;
        }

        protected async Task SearchIcons()
        {
            await Task.Delay(100);
            var iconResponse = await IconService.SearchPaginationAsync(new()
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

        protected async Task CloseAddDialog()
        {
            showAddDialog = false;
            await SearchIcons();
        }

        protected void OpenAddMultiIconDialog()
        {
            showAddMultiIconsDialog = true;
            showAddDialog = false;
        }

        protected async Task CloseAddMultiIconDialog()
        {
            showAddMultiIconsDialog = false;
            await SearchIcons();
        }

        protected void OpenIconMenu(IconModel icon)
        {
            menuIcon = icon;
        }

        protected async Task DeleteIcon(IconModel? icon)
        {
            if (icon == null)
                return;

            await IconService.DeleteAsync(icon.Id);
            await SearchIcons();
        }

        protected void OpenIconLink(IconModel? icon)
        {
            if (icon == null)
                return;

            Navigation.NavigateTo(icon.Url, true);
        }
    }
}