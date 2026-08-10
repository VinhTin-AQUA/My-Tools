using System.Runtime.InteropServices;
using System.Text;
using QuickTools.Windows.Modules.WebUI.Methods;

namespace QuickTools.Windows.Modules.WebUI
{
    /*
     * Quản lý toàn bộ thư viện WebUI
     */
    public static class WebUIWindow
    {
        public static UIntPtr CreateNewWindow()
        {
            var window = WindowManagementMethods.webui_new_window();
            return window;
        }

        public static void SetConfig(webui_config config, bool status)
        {
            ConfigMethods.webui_set_config(config, status);
        }
        
        public static void SetEventBlocking(UIntPtr window, bool status)
        {
            ConfigMethods.webui_set_event_blocking(window, status);
        }
        
        public static bool WebuiSetRootFolder(UIntPtr window, string path)
        {
            var r = FileAndFolderMethods.webui_set_root_folder(window, path);
            return r;
        }
        
        public static bool Show(UIntPtr window, string content, webui_browser browser = webui_browser.AnyBrowser)
        {
            if (browser == webui_browser.AnyBrowser)
                return WindowManagementMethods.webui_show(window, content);
            return WindowManagementMethods.webui_show_browser(window, content, (UIntPtr)browser);
        }
        
        public static void WebUIWait()
        {
            WindowManagementMethods.webui_wait();
        }
        
        public static IntPtr GetHWND(UIntPtr window)
        {
            return WindowManagementMethods.webui_get_hwnd(window);
        }

        public static void WebuiSetSize(UIntPtr window, uint width, uint height)
        {
            WindowSettingMethods.webui_set_size(window, width, height);
        }
        
        public static void WebuiSetMinimumSize(UIntPtr window, uint width, uint height)
        {
            WindowSettingMethods.webui_set_minimum_size(window, width, height);
        }
        
        public static void WebuiSetPosition(UIntPtr window, uint width, uint height)
        {
            WindowSettingMethods.webui_set_position(window, width, height);
        }
        
        public static void WebuiSetCenter(UIntPtr window)
        {
            WindowSettingMethods.webui_set_center(window);
        }
    }
}
