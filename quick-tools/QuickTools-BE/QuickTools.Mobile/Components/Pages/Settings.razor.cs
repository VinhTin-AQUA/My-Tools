using Microsoft.AspNetCore.Components;

namespace QuickTools.Mobile.Components.Pages
{
    public partial class Settings : ComponentBase
    {
        private void SetLight()
        {
            ThemeService.SetLight();
        }

        private void SetDark()
        {
            ThemeService.SetDark();
        }
        
        // private string SelectedTheme { get; set; } = "light";
        private bool IsLightTheme => ThemeService.CurrentTheme.Name == Themes.Themes.Light.Name;
        private bool IsDarkTheme => ThemeService.CurrentTheme.Name == Themes.Themes.Dark.Name;
    }
}