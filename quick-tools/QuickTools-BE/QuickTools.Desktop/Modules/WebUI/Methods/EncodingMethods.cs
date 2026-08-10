using System.Runtime.InteropServices;

namespace QuickTools.Windows.Modules.WebUI.Methods
{
    public static class EncodingMethods
    {
        [DllImport(ModuleConstants.WebUIBinLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr webui_encode(string str);

        [DllImport(ModuleConstants.WebUIBinLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr webui_decode(string str);
    }
}