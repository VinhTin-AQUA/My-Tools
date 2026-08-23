using QuickTools.Windows.Helpers;
using WebUISharp;

namespace QuickTools.Windows.Handlers.FolderHandlers
{
    public static class FolderHandler
    {
        public static void OpenFolder(UIntPtr window, UIntPtr event_type, IntPtr element, UIntPtr event_number, UIntPtr bind_id)
        {
            IntPtr dataPtr = WebUI.InterfaceGetStringAt(window, event_number, UIntPtr.Zero);
            string folderPath = WebUI.PtrToString(dataPtr);
            
            Task.Run(() =>
            {
                FolderHelper.OpenFolder(folderPath);
            });
        }
    }
}