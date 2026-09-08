using Microsoft.AspNetCore.Components;

namespace QuickTools.MobileTemplate.Components.Pages.Settings
{
    public partial class MongoConfig : ComponentBase
    {
        
        private string MongoConnectionString { get; set; } = string.Empty;

        private string MongoDatabaseName { get; set; } = string.Empty;

        private bool IsSavingMongoDb { get; set; }

        private bool MongoSaveSucceeded { get; set; }

        private string? MongoStatusMessage { get; set; }


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
                // var response =
                //     await WebUIService.CallJsonAsync<WebUIResponse<MongoDBSetting>>(
                //         "setMongoDBSetting",
                //         new
                //         {
                //             connectionString = MongoConnectionString,
                //             databaseName = MongoDatabaseName
                //         });
                //
                // MongoSaveSucceeded = true;
                // MongoStatusMessage = response.Description;
            }
            catch (Exception ex)
            {
                // MongoSaveSucceeded = false;
                // MongoStatusMessage = ex.Message;
            }
            finally
            {
                // IsSavingMongoDb = false;
            }
        }
    }
}