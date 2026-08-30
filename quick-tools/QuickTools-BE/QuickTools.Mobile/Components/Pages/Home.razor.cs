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
            new() { Name = "Upscale Image", IconClass = "fa-regular fa-circle-up", Url = "image-upscaler" },
            new() { Name = "Compress Image", IconClass = "fa-solid fa-compress", Url = "compress-image" },
            new() { Name = "Settings", IconClass = "fa-solid fa-gear", Url = "settings" },
            new() { Name = "Orders", IconClass = "fas fa-shopping-cart", Url = "orders" },
            new() { Name = "Reports", IconClass = "fas fa-file-alt", Url = "reports" },
            new() { Name = "Profile", IconClass = "fas fa-user-circle", Url = "profile" },
            new() { Name = "Notifications", IconClass = "fas fa-bell", Url = "notifications" },
            new() { Name = "Activity Log", IconClass = "fas fa-history", Url = "activity-log" },
            new() { Name = "Support", IconClass = "fas fa-headset", Url = "support" },
            new() { Name = "Permissions", IconClass = "fas fa-lock", Url = "permissions" },
            new() { Name = "Analytics", IconClass = "fas fa-chart-line", Url = "analytics" },
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