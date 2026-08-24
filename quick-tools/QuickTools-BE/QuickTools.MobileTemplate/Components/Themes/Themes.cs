namespace QuickTools.MobileTemplate.Components.Themes
{
    public static class Themes
    {
        public static AppTheme Light => new()
        {
            Name = "Light",

            Colors = new ThemeColors
            {
                Primary = "#2563EB",
                PrimaryHover = "#1D4ED8",

                Secondary = "#64748B",

                Background = "#F8FAFC",
                Surface = "#FFFFFF",

                TextPrimary = "#0F172A",
                TextSecondary = "#64748B",
                TextDisabled = "#94A3B8",

                Border = "#E2E8F0",

                Success = "#16A34A",
                Warning = "#D97706",
                Error = "#DC2626",
                Info = "#0284C7"
            }
        };

        public static AppTheme Dark => new()
        {
            Name = "Dark",

            Colors = new ThemeColors
            {
                Primary = "#60A5FA",
                PrimaryHover = "#3B82F6",

                Secondary = "#94A3B8",

                Background = "#0F172A",
                Surface = "#1E293B",

                TextPrimary = "#F8FAFC",
                TextSecondary = "#94A3B8",
                TextDisabled = "#64748B",

                Border = "#334155",

                Success = "#4ADE80",
                Warning = "#FBBF24",
                Error = "#F87171",
                Info = "#38BDF8"
            }
        };
    }
}