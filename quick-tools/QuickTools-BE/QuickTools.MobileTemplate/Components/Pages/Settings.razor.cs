using QuickTools.MobileTemplate.Components.Themes;

namespace QuickTools.MobileTemplate.Components.Pages
{
    public partial class Settings
    {
        private void SetLight()
        {
            ThemeService.SetLight();
        }

        private void SetDark()
        {
            ThemeService.SetDark();
        }
    }
}