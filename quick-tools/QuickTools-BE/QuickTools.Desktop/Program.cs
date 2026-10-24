using QuickTools.Windows.Handlers.FolderHandlers;
using QuickTools.Windows.Handlers.GoogleSheetPasswordManagerhandlers;
using QuickTools.Windows.Handlers.IloveimgHandlers;
using QuickTools.Windows.Handlers.LibcaesiumHandlers;
using WebUISharp;

namespace QuickTools.Windows
{
    internal class Program
    {
        private static void Main()
        {
            // Tạo window
            var window = WebUI.NewWindow();
            
            // WebUIBinder.Bind(window, "longTask", ExampleHandlers.LongTaskHandler);
            // WebUIBinder.Bind(window, "getData", ExampleHandlers.GetDataHandler);
            // WebUIBinder.Bind(window, "sendData", ExampleHandlers.SendDataHandler);
            // WebUIBinder.Bind(window, "requestData", ExampleHandlers.RequestDataHandler);
            // WebUIBinder.BindAsyncFunction(window, "asyncFunction", ExampleHandlers.MyAsyncFunction);
            
            GoogleSheetPasswordManagerHandler.Init();
            
            WebUIBinder.BindAsyncFunctionWithNullValue(window, "upscaleImage", UpscaleImageHandler.UpscaleImage);
            WebUIBinder.BindAsyncFunctionWithNullValue(window, "libcaesiumCompressImage", LibcaesiumHandler.CompressImage);
            
            WebUIBinder.Bind(window, "openFolder", FolderHandler.OpenFolder);
            
            // GoogleSheetPasswordManagerHandler
            WebUIBinder.BindAsyncFunctionWithNullValue(window, "GoogleSheetPasswordManagerHandler_GetAll", GoogleSheetPasswordManagerHandler.GetAll);
            WebUIBinder.BindAsyncFunctionWithNullValue(window, "GoogleSheetPasswordManagerHandler_Create", GoogleSheetPasswordManagerHandler.Create);
            WebUIBinder.BindAsyncFunctionWithNullValue(window, "GoogleSheetPasswordManagerHandler_Update", GoogleSheetPasswordManagerHandler.Update);
            WebUIBinder.BindAsyncFunctionWithNullValue(window, "GoogleSheetPasswordManagerHandler_Delete", GoogleSheetPasswordManagerHandler.Delete);

            // GoogleSheetPasswordManagerConfigHandler
            WebUIBinder.BindAsyncFunctionWithNullValue(window, "GoogleSheetPasswordManagerConfigHandler_SetKey", GoogleSheetPasswordManagerConfigHandler.SetKey);
            WebUIBinder.BindAsyncFunctionWithNullValue(window, "GoogleSheetPasswordManagerConfigHandler_VerifyKey", GoogleSheetPasswordManagerConfigHandler.VerifyKey);
            WebUIBinder.BindAsyncFunctionWithNullValue(window, "GoogleSheetPasswordManagerConfigHandler_SetUrl", GoogleSheetPasswordManagerConfigHandler.SetUrl);
            WebUIBinder.BindAsyncFunctionWithNullValue(window, "GoogleSheetPasswordManagerConfigHandler_GetConfig", GoogleSheetPasswordManagerConfigHandler.GetConfig);
            
            
            // Cấu hình async
            WebUI.SetConfig(WebuiConfig.asynchronous_response, true);
            WebUI.SetEventBlocking(window, false);
            
            // Set root folder (thư mục chứa file HTML và các assets)
            string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            WebUI.SetRootFolder(window, rootPath);
            WebUI.SetMinimumSize(window, 1000, 600);
            WebUI.SetSize(window, 1000, 600);
            WebUI.SetCenter(window);
            Console.WriteLine($"Root folder set to: {rootPath}");
            
            // Show window
            // WebUIManager.Show(window, "/wwwroot/index.html");
            WebUI.Show(window, "index.html");

            // Wait
            WebUI.Wait();
        }
    }
}