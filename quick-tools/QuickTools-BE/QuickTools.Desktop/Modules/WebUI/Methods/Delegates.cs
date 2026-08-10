using System.Runtime.InteropServices;

namespace QuickTools.Windows.Modules.WebUI.Methods
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void webui_callback_t(ref webui_event_t e);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.I1)]
    public delegate bool webui_close_handler_t(UIntPtr window);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr webui_file_handler_t(string filename, ref int length);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr webui_file_handler_window_t(UIntPtr window, string filename, ref int length);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void webui_interface_callback_t(UIntPtr window, UIntPtr event_type, IntPtr element,
        UIntPtr event_number, UIntPtr bind_id);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void webui_logger_t(UIntPtr level, IntPtr log, IntPtr user_data);
}