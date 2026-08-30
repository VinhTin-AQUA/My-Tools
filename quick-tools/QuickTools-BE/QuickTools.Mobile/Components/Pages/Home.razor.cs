using QuickTools.Services.IP;
using QuickTools.Services.Models.IP;

namespace QuickTools.Mobile.Components.Pages
{
    public partial class Home 
    {
        private string searchTerm = string.Empty;
        private bool isLoading = true;
        private string errorMessage = "";
        
        private IpApiResponse? ipInfo;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                ipInfo = await IPService.GetIpInformationAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Không lấy được thông tin IP: {ex.Message}");
                errorMessage = ex.ToString();
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                isLoading = false;
            }
        }

        private readonly List<MenuItem> allMenus = new()
        {
            new MenuItem { Name = "Upscale Image", IconClass = "fa-regular fa-circle-up", Url = "image-upscaler" },
            new MenuItem { Name = "Compress Image", IconClass = "fa-solid fa-compress", Url = "compress-image" },
            new MenuItem { Name = "Settings", IconClass = "fa-solid fa-gear", Url = "settings" },
            new MenuItem { Name = "Orders", IconClass = "fas fa-shopping-cart" },
            new MenuItem { Name = "Reports", IconClass = "fas fa-file-alt" },
            new MenuItem { Name = "Settings", IconClass = "fas fa-cog" },
            new MenuItem { Name = "Profile", IconClass = "fas fa-user-circle" },
            new MenuItem { Name = "Notifications", IconClass = "fas fa-bell" },
            new MenuItem { Name = "Activity Log", IconClass = "fas fa-history" },
            new MenuItem { Name = "Support", IconClass = "fas fa-headset" },
            new MenuItem { Name = "Permissions", IconClass = "fas fa-lock" },
            new MenuItem { Name = "Analytics", IconClass = "fas fa-chart-line" }
        };

        private List<MenuItem> FilteredMenus => string.IsNullOrWhiteSpace(searchTerm)
            ? allMenus
            : allMenus.Where(m => m.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();

        public class MenuItem
        {
            public string Name { get; set; } = string.Empty;
            public string IconClass { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
        }
    }
}