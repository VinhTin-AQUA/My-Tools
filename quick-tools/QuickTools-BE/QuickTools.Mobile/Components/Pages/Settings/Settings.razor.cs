using Microsoft.AspNetCore.Components;

namespace QuickTools.Mobile.Components.Pages.Settings
{
    public partial class Settings : ComponentBase
    {
        private readonly List<SettingMenuItem> SettingMenus =
        [
            new()
            {
                Title = "Theme", 
                Description = "Customize the appearance of the application.", 
                Url = "/settings/theme",
                Icon = "fa-solid fa-palette"
            },
            new()
            {
                Title = "MongoDB", 
                Description = "Configure MongoDB connection and database settings.",
                Url = "/settings/mongoconfig", 
                Icon = "fa-solid fa-database"
            }
        ];

        private class SettingMenuItem
        {
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
        }
    }
}