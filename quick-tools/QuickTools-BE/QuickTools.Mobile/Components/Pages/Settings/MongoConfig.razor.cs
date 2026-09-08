using Microsoft.AspNetCore.Components;
using QuickTools.Services.LocalStorages;

namespace QuickTools.Mobile.Components.Pages.Settings
{
    public partial class MongoConfig : ComponentBase
    {
        [Inject] protected SecureStorageService SecureStorageService { get; set; } = default!;
        
        private readonly string _mongoConfigKey = "MongoConfigKey";
        
        public string MongoConnectionString { get; set; } = string.Empty;

        public string MongoDatabaseName { get; set; } = string.Empty;

        private bool IsSavingMongoDb { get; set; }

        private bool MongoSaveSucceeded { get; set; }

        private string? MongoStatusMessage { get; set; }
        
        
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var mongoConfig = await SecureStorageService.LoadAsync<MongoConfig>(_mongoConfigKey);

                if (mongoConfig != null)
                {
                    MongoConnectionString = mongoConfig.MongoConnectionString;
                    MongoDatabaseName = mongoConfig.MongoDatabaseName;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
            }
        }
        
        private async Task SaveMongoDb()
        {
            if (IsSavingMongoDb)
                return;

            if (string.IsNullOrWhiteSpace(MongoConnectionString))
            {
                MongoSaveSucceeded = false;
                MongoStatusMessage = "Connection String is required.";
                return;
            }

            if (string.IsNullOrWhiteSpace(MongoDatabaseName))
            {
                MongoSaveSucceeded = false;
                MongoStatusMessage = "Database Name is required.";
                return;
            }

            IsSavingMongoDb = true;
            MongoStatusMessage = null;

            try
            {
                var check = await SecureStorageService.SaveAsync<MongoConfigModel>(_mongoConfigKey, new()
                {
                    MongoConnectionString = MongoConnectionString,
                    MongoDatabaseName = MongoDatabaseName
                });
                
                MongoSaveSucceeded = check;
                MongoStatusMessage = check ? "MongoDB saved successfully!" : "MongoDB not saved successfully!";;
            }
            catch (Exception ex)
            {
                MongoSaveSucceeded = false;
                MongoStatusMessage = ex.Message;
            }
            finally
            {
                IsSavingMongoDb = false;
            }
        }
    }

    public class MongoConfigModel
    {
        public string MongoConnectionString { get; set; } = string.Empty;
        public string MongoDatabaseName { get; set; } = string.Empty;
    } 
}