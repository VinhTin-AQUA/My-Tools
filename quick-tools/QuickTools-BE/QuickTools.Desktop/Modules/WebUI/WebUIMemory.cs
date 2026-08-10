using QuickTools.Windows.Modules.WebUI.Methods;

namespace QuickTools.Windows.Modules.WebUI
{
    public static class WebUIMemory
    {
        public static IntPtr Malloc(UIntPtr size)
        {
            return MemoryMethods.webui_malloc(size);
        }

        public static void Free(IntPtr ptr)
        {
            MemoryMethods.webui_free(ptr);
        }

        public static void MemCpy(IntPtr dest, IntPtr src, UIntPtr count)
        {
            MemoryMethods.webui_memcpy(dest, src, count);
        }
    }
}