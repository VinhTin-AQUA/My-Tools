using QuickTools.Services.LocalStorages;

namespace QuickTools.Windows.AppSingletons
{
    public static class JsonStorageSingleton
    {
        static JsonStorageSingleton()
        {
            Instance = new JsonStorage();
            GoogleSheetConfigInstance = new JsonStorage("GoogleSheetConfig.json");
        }

        public static JsonStorage Instance { get; }
        public static JsonStorage GoogleSheetConfigInstance { get; }
    }
}