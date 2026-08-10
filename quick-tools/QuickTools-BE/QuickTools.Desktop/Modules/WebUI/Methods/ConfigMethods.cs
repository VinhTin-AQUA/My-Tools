using System.Runtime.InteropServices;

namespace QuickTools.Windows.Modules.WebUI.Methods
{
    public static class ConfigMethods
    {
        [DllImport(ModuleConstants.WebUIBinLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern void webui_set_config(webui_config option, [MarshalAs(UnmanagedType.I1)] bool status);

        [DllImport(ModuleConstants.WebUIBinLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern void webui_set_event_blocking(UIntPtr window, [MarshalAs(UnmanagedType.I1)] bool status);

    }
}