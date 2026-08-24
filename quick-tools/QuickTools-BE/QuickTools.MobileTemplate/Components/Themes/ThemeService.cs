namespace QuickTools.MobileTemplate.Components.Themes
{
    public class ThemeService
    {
        public AppTheme CurrentTheme { get; private set; }

        public event Action? ThemeChanged;

        public ThemeService()
        {
            CurrentTheme = Themes.Light;
        }

        public void SetTheme(AppTheme theme)
        {
            CurrentTheme = theme;

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