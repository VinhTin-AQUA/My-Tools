

using System.Text.Json;
using QuickTools.Core.DTOs.Icons;
using QuickTools.Core.Models;
using QuickTools.Core.Responses;
using QuickTools.Windows.AppSingletons;
using QuickTools.Windows.Handlers.IconHandlers;
using WebUISharp;

namespace QuickTools.Windows.Handlers.MongoSettingHandlers
{
    public static class MongoSettingHandler
    {
        private static readonly string mongoDBSettingKey = "mongoDBSetting";
        
        public static async Task GetMongoDBSetting(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                };
   
                var jsonStorageService = JsonStorageSingleton.Instance;
                var mongoDBSetting = await jsonStorageService.GetItemAsync<MongoDBSetting>(mongoDBSettingKey);
                
                string json = JsonSerializer.Serialize(mongoDBSetting, options);
                WebUI.InterfaceSetResponse(window, event_number, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ HandleGetIconsEvent error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                WebUI.InterfaceSetResponse(window, event_number, JsonSerializer.Serialize(new MongoDBSetting()));
            }
        }
        
        public static async Task SetMongoDBSetting(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            try
            {
                var dataPtr = WebUI.InterfaceGetStringAt(window, event_number, UIntPtr.Zero);
                var jsonData = WebUI.PtrToString(dataPtr);
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                };
                
                var setMongoDBSettingRequest = JsonSerializer.Deserialize<MongoDBSetting>(jsonData, options);

                if (setMongoDBSettingRequest == null)
                { 
                    WebUI.InterfaceSetResponse(window, event_number, JsonSerializer.Serialize(new WebUIResponse<MongoDBSetting>
                    {
                        Action = "SetMongoDBSetting",
                        Data = null,
                        Description = "setMongoDBSettingRequest is null",
                        Success = false,
                        Title = "Cannot set MongoDBSetting"
                    }));
                    return;
                }
   
                var jsonStorageService = JsonStorageSingleton.Instance;
                var r = await jsonStorageService.InsertItemAsync<MongoDBSetting>(mongoDBSettingKey, new()
                {
                    ConnectionString = setMongoDBSettingRequest.ConnectionString,
                    DatabaseName = setMongoDBSettingRequest.DatabaseName,
                });
                
                string json = JsonSerializer.Serialize(new WebUIResponse<MongoDBSetting>
                {
                    Action = "SetMongoDBSetting",
                    Data = setMongoDBSettingRequest,
                    Description = "setMongoDBSettingRequest is null",
                    Success = false,
                    Title = "Cannot set MongoDBSetting"
                }, options);
                WebUI.InterfaceSetResponse(window, event_number, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ HandleGetIconsEvent error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                WebUI.InterfaceSetResponse(window, event_number, JsonSerializer.Serialize(new WebUIResponse<MongoDBSetting>
                {
                    Action = "SetMongoDBSetting",
                    Data = null,
                    Description = ex.Message,
                    Success = false,
                    Title = "Cannot set MongoDBSetting"
                }));
            }
        }
    }
}