using System.Runtime.InteropServices;

namespace QuickTools.Windows.Modules.WebUI.Methods
{
    public static class IconMethods
    {
        [DllImport(ModuleConstants.WebUIBinLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern void webui_set_icon(UIntPtr window, string icon, string icon_type);

        [DllImport(ModuleConstants.WebUIBinLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern void webui_set_icon_file(UIntPtr window, string path);
    }
}