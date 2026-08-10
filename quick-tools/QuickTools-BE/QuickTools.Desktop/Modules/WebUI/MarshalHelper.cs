using System.Runtime.InteropServices;

namespace QuickTools.Windows.Modules.WebUI
{
    public static class MarshalHelper
    {
        public static string PtrToString(IntPtr ptr)
        {
            return Marshal.PtrToStringUTF8(ptr) ?? string.Empty;
        }
    }
}