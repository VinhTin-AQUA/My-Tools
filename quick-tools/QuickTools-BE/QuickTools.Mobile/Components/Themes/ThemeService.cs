namespace QuickTools.Mobile.Components.Themes
{
    public class ThemeService
    {
        private string _themKey = "theme";
        public AppTheme CurrentTheme { get; private set; }

        public event Action? ThemeChanged;

        public ThemeService()
        {
            var theme = Preferences.Default.Get(_themKey, Themes.Dark.Name);

            switch (theme)
            {
                case var t when t == Themes.Dark.Name:
                    CurrentTheme = Themes.Dark;
                    break;

                case var t when t == Themes.Light.Name:
                    CurrentTheme = Themes.Light;
                    break;

                default:
                    CurrentTheme = Themes.Dark;
                    break;
            }
        }

        public void SetTheme(AppTheme theme)
        {
            CurrentTheme = theme;
            Preferences.Default.Set(_themKey, theme.Name);
            ThemeChanged?.Invoke();
        }

        public void SetLight()
        {
            SetTheme(Themes.Light);
        }

        public void SetDark()
        {
            SetTheme(Themes.Dark);
        }

        public string GetCssVariables()
        {
            var colors = CurrentTheme.Colors;

            return $"""
                    --color-primary: {colors.Primary};
                    --color-primary-hover: {colors.PrimaryHover};

                    --color-secondary: {colors.Secondary};

                    --color-background: {colors.Background};
                    --color-surface: {colors.Surface};

                    --color-text-primary: {colors.TextPrimary};
                    --color-text-secondary: {colors.TextSecondary};
                    --color-text-disabled: {colors.TextDisabled};

                    --color-border: {colors.Border};

                    --color-success: {colors.Success};
                    --color-warning: {colors.Warning};
                    --color-error: {colors.Error};
                    --color-info: {colors.Info};
                    """;
        }
    }
}