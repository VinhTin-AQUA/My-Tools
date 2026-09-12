using QuickTools.Core.Constants;
using QuickTools.Core.Models;
using QuickTools.Services.Icons;
using QuickTools.Services.MongoDB;

namespace QuickTools.Windows.AppSingletons
{
    public static class IconServiceSingleton
    {
        // Static constructor - tự động chạy 1 lần duy nhất
        // static IconServiceSingleton()
        // {
        //     var connectionString = "mongodb+srv://tinhovinh_db_user:FBFEDtBwoDoL6Byg@cluster0.v0h03ni.mongodb.net/";
        //     var databaseName = "QuickTools";
        //
        //     var context = new MongoDbContext(connectionString, databaseName);
        //     Instance = new IconService(context);
        // }

        public static IIconService? _instance { get; set; } = null;

        public static async Task<IIconService?> GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            var mongoDbSetting =
                await JsonStorageSingleton.Instance.GetItemAsync<MongoDBSetting>(MongoDBSettingConstants
                    .MongoDBSettingKey);
            
            var connectionString = mongoDbSetting.ConnectionString;
            var databaseName = mongoDbSetting.DatabaseName ;

            bool checkConnection = false;
            MongoDbContext? context = null;
            try
            {
                context = new MongoDbContext(connectionString, databaseName);
                (var connected, var message) = await context.CheckConnectionAsync();
                checkConnection =  connected;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ MongoDB connection failed: {ex.Message}");
                checkConnection = false;
            }
            
            if (!checkConnection || context == null)
            {
                return null;
            }
            _instance = new IconService(context);
                
            return _instance;
        }
    }
}