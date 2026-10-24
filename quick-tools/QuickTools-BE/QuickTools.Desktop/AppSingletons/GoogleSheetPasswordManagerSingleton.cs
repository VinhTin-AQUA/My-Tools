using QuickTools.Services.PasswordManager;

namespace QuickTools.Windows.AppSingletons
{
    public static class GoogleSheetPasswordManagerSingleton
    {
        static GoogleSheetPasswordManagerSingleton()
        {
            Instance = new GoogleSheetPasswordManagerService();
        }

        public static GoogleSheetPasswordManagerService Instance { get; }
    }
}