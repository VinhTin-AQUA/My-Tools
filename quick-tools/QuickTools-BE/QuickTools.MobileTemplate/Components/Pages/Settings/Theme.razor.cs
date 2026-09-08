using Microsoft.AspNetCore.Components;

namespace QuickTools.MobileTemplate.Components.Pages.Settings
{
    public partial class Theme : ComponentBase
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