using QuickTools.Services.LocalStorages;

namespace QuickTools.Windows.AppSingletons
{
    public static class JsonStorageSingleton
    {
        static JsonStorageSingleton()
        {
            Instance = new JsonStorage();
        }

        public static JsonStorage Instance { get; }
    }
}