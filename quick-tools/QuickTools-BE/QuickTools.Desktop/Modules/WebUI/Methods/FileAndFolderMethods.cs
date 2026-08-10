using System.Runtime.InteropServices;

namespace QuickTools.Windows.Modules.WebUI.Methods
{
    public static class FileAndFolderMethods
    {
        [DllImport(ModuleConstants.WebUIBinLib, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool webui_set_root_folder(UIntPtr window, string path);

        [DllImport(ModuleConstants.WebUIBinLib, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool webui_set_default_root_folder(string path);

        [DllImport(ModuleConstants.WebUIBinLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern void webui_set_file_handler(UIntPtr window, IntPtr handler);

        [DllImport(ModuleConstants.WebUIBinLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern void webui_set_file_handler_window(UIntPtr window, IntPtr handler);

        [DllImport(ModuleConstants.WebUIBinLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern void
            webui_interface_set_response_file_handler(UIntPtr window, IntPtr response, int length);

        [DllImport(ModuleConstants.WebUIBinLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr webui_get_mime_type(string file);
    }
}