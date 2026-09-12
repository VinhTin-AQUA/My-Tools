using Microsoft.AspNetCore.Components;
using QuickTools.Core.Models;
using QuickTools.Mobile.Constants;
using QuickTools.Mobile.Services.Interfaces;

namespace QuickTools.Mobile.Components.Pages.Settings
{
    public partial class MongoConfig : ComponentBase
    {
        [Inject] protected ISecureStorageService SecureStorageService { get; set; } = default!;
        
        public string MongoConnectionString { get; set; } = string.Empty;

        public string MongoDatabaseName { get; set; } = string.Empty;

        private bool IsSavingMongoDb { get; set; }

        private bool MongoSaveSucceeded { get; set; }

        private string? MongoStatusMessage { get; set; }
        
        
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var mongoConfig = await SecureStorageService.LoadAsync<MongoDBSetting>(AppConstants.MongoConfigKey);

                if (mongoConfig != null)
                {
                    MongoConnectionString = mongoConfig.ConnectionString;
                    MongoDatabaseName = mongoConfig.DatabaseName;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
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
                var check = await SecureStorageService.SaveAsync<MongoDBSetting>(AppConstants.MongoConfigKey, new()
                {
                    ConnectionString = MongoConnectionString,
                    DatabaseName = MongoDatabaseName
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
}