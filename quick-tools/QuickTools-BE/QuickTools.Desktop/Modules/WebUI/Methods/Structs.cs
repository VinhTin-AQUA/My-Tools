using System.Runtime.InteropServices;

namespace QuickTools.Windows.Modules.WebUI.Methods
{
    [StructLayout(LayoutKind.Sequential)]
    public struct webui_event_t
    {
        public UIntPtr window;
        public UIntPtr event_type;
        public IntPtr element;
        public UIntPtr event_number;
        public UIntPtr bind_id;
        public UIntPtr client_id;
        public UIntPtr connection_id;
        public IntPtr cookies;

        // Helper methods
        public string? GetElement()
        {
            return Marshal.PtrToStringAnsi(element);
        }

        public string? GetCookies()
        {
            return Marshal.PtrToStringAnsi(cookies);
        }
    }
}