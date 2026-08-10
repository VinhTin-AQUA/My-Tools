using System.Runtime.InteropServices;

namespace QuickTools.Windows.Modules.WebUI.Methods
{
    public static class SendDataMethods
    {
        [DllImport(ModuleConstants.WebUIBinLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern void webui_send_raw(UIntPtr window, string function, IntPtr raw, UIntPtr size);

        [DllImport(ModuleConstants.WebUIBinLib, CallingConvention = CallingConvention.Cdecl)]
        public static extern void webui_send_raw_client(ref webui_event_t e, string function, IntPtr raw, UIntPtr size);

    }
}