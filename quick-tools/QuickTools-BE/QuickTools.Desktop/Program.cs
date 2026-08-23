using QuickTools.Windows.Handlers.FolderHandlers;
using QuickTools.Windows.Handlers.IloveimgHandlers;
using WebUISharp;

namespace QuickTools.Windows
{
    internal class Program
    {
        private static void Main()
        {
            // NativeLibraryManager.Initialize();

            // Tạo window
            var window = WebUI.NewWindow();
            
            // WebUIBinder.Bind(window, "longTask", ExampleHandlers.LongTaskHandler);
            // WebUIBinder.Bind(window, "getData", ExampleHandlers.GetDataHandler);
            // WebUIBinder.Bind(window, "sendData", ExampleHandlers.SendDataHandler);
            // WebUIBinder.Bind(window, "requestData", ExampleHandlers.RequestDataHandler);
            // WebUIBinder.BindAsyncFunction(window, "asyncFunction", ExampleHandlers.MyAsyncFunction);
            
            WebUIBinder.BindAsyncFunctionWithNullValue(window, "upscaleImage", UpscaleImageHandler.UpscaleImage);
            WebUIBinder.Bind(window, "openFolder", FolderHandler.OpenFolder);

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