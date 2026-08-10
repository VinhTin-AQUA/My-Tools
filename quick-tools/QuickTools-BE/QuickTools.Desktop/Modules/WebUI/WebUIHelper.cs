using QuickTools.Windows.Modules.WebUI.Methods;

namespace QuickTools.Windows.Modules.WebUI
{
    public static class WebUIHelper
    {
        public static string? GetUrl(UIntPtr window)
        {
            return MarshalHelper.PtrToString(NavigationMethods.webui_get_url(window));
        }

        public static string? GetLastErrorMessage()
        {
            return MarshalHelper.PtrToString(ErrorMethods.webui_get_last_error_message());
        }

        public static string? GetMimeType(string file)
        {
            return MarshalHelper.PtrToString(FileAndFolderMethods.webui_get_mime_type(file));
        }

        public static string? Encode(string str)
        {
            return MarshalHelper.PtrToString(EncodingMethods.webui_encode(str));
        }

        public static string? Decode(string str)
        {
            return MarshalHelper.PtrToString(EncodingMethods.webui_decode(str));
        }

        public static string? StartServer(UIntPtr window, string content)
        {
            return MarshalHelper.PtrToString(WindowManagementMethods.webui_start_server(window, content));
        }

    }
}