namespace QuickTools.Mobile.Components.Themes
{
    public static class Themes
    {
        public static AppTheme Light => new()
        {
            Name = "Light",

            Colors = new ThemeColors
            {
                // Brand
                Primary = "#4F46E5",
                PrimaryHover = "#4338CA",

                // Neutral
                Secondary = "#64748B",

                // Background
                Background = "#F8FAFC",
                Surface = "#FFFFFF",

                // Text
                TextPrimary = "#0F172A",
                TextSecondary = "#475569",
                TextDisabled = "#94A3B8",

                // Border
                Border = "#E2E8F0",

                // Semantic
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
                // Brand
                Primary = "#818CF8",
                PrimaryHover = "#A5B4FC",

                // Neutral
                Secondary = "#94A3B8",

                // Background
                Background = "#0B1120",
                Surface = "#151F32",

                // Text
                TextPrimary = "#F8FAFC",
                TextSecondary = "#CBD5E1",
                TextDisabled = "#64748B",

                // Border
                Border = "#263449",

                // Semantic
                Success = "#4ADE80",
                Warning = "#FBBF24",
                Error = "#F87171",
                Info = "#38BDF8"
            }
        };
    }
}