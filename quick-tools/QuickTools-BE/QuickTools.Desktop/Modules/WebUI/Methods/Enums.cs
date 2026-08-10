using System.Runtime.InteropServices;
using System.Text;

namespace QuickTools.Windows.Modules.WebUI.Methods
{
    public enum webui_browser : uint
    {
        NoBrowser = 0,
        AnyBrowser = 1,
        Chrome,
        Firefox,
        Edge,
        Safari,
        Chromium,
        Opera,
        Brave,
        Vivaldi,
        Epic,
        Yandex,
        ChromiumBased,
        Webview
    }

    public enum webui_runtime : uint
    {
        None = 0,
        Deno,
        NodeJS,
        Bun
    }

    public enum webui_event_type : uint
    {
        WEBUI_EVENT_DISCONNECTED = 0,
        WEBUI_EVENT_CONNECTED,
        WEBUI_EVENT_MOUSE_CLICK,
        WEBUI_EVENT_NAVIGATION,
        WEBUI_EVENT_CALLBACK
    }

    public enum webui_config
    {
        show_wait_connection = 0,
        ui_event_blocking,
        folder_monitor,
        multi_client,
        use_cookies,
        asynchronous_response
    }

    public enum webui_logger_level : uint
    {
        WEBUI_LOGGER_LEVEL_DEBUG = 0,
        WEBUI_LOGGER_LEVEL_INFO,
        WEBUI_LOGGER_LEVEL_ERROR
    }
}