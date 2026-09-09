using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace QuickTools.MobileTemplate.Components.Pages.IconMemes
{
    public partial class IconMeme : ComponentBase
    {
        protected string searchTerm = "";
        protected bool showAddDialog;
        protected bool showAddMultiIconsDialog;
        protected int pageIndex = 1;
        protected int pageSize = 20;
        protected List<IconModel> icons = new();
        protected bool connected = true;
        protected IconModel? menuIcon;

        [Inject] protected NavigationManager Navigation { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await CheckConnection();

            if (connected)
                await SearchIcons();
        }

        protected async Task CheckConnection()
        {
            await Task.Delay(100);
            connected = true;
        }

        protected async Task SearchIcons()
        {
            await Task.Delay(100);

            // TODO: API
            // icons = await Api.GetIconsAsync(searchTerm, pageIndex, pageSize);
        }

        protected void OnSearchInput(ChangeEventArgs e)
        {
            searchTerm = e.Value?.ToString() ?? "";
        }

        protected async Task OnSearchKeyUp(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                pageIndex = 1;
                await SearchIcons();
            }
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

            await Task.Delay(200);
            await SearchIcons();
        }

        protected void OpenIconLink(IconModel? icon)
        {
            if (icon == null)
                return;

            Navigation.NavigateTo(icon.Url, true);
        }
    }

    // Models
    public class IconModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public IconType Type { get; set; }
        public string Url { get; set; } = "";
    }

    public enum IconType
    {
        Gift = 0,
        Image = 1
    }
}