using QuickTools.Windows.Handlers.FolderHandlers;
using QuickTools.Windows.Handlers.IloveimgHandlers;
using QuickTools.Windows.Modules.LoaderManager;
using QuickTools.Windows.Modules.WebUI;
using QuickTools.Windows.Modules.WebUI.Methods;

namespace QuickTools.Windows
{
    internal class Program
    {
        private static void Main()
        {
            NativeLibraryManager.Initialize();

            // Tạo window
            var window = WebUIWindow.CreateNewWindow();
            
            // InterfaceBinder.Bind(window, "longTask", ExampleHandlers.LongTaskHandler);
            // InterfaceBinder.Bind(window, "getData", ExampleHandlers.GetDataHandler);
            // InterfaceBinder.Bind(window, "sendData", ExampleHandlers.SendDataHandler);
            // InterfaceBinder.Bind(window, "requestData", ExampleHandlers.RequestDataHandler);
            // InterfaceBinder.BindAsyncFunction(window, "asyncFunction", ExampleHandlers.MyAsyncFunction);
            
            InterfaceBinder.BindAsyncFunctionWithNullValue(window, "upscaleImage", UpscaleImageHandler.UpscaleImage);
            InterfaceBinder.Bind(window, "openFolder", FolderHandler.OpenFolder);

            // Cấu hình async
            WebUIWindow.SetConfig(webui_config.asynchronous_response, true);
            WebUIWindow.SetEventBlocking(window, false);
            
            // Set root folder (thư mục chứa file HTML và các assets)
            string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            WebUIWindow.WebuiSetRootFolder(window, rootPath);
            WebUIWindow.WebuiSetMinimumSize(window, 1000, 600);
            WebUIWindow.WebuiSetSize(window, 1000, 600);
            WebUIWindow.WebuiSetCenter(window);
            Console.WriteLine($"Root folder set to: {rootPath}");
            
            // Show window
            // WebUIManager.Show(window, "/wwwroot/index.html");
            WebUIWindow.Show(window, "index.html");

            // Wait
            WebUIWindow.WebUIWait();
        }
    }
}