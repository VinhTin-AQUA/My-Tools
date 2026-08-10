using QuickTools.Windows.Helpers;
using QuickTools.Windows.Modules.WebUI;
using QuickTools.Windows.Modules.WebUI.Methods;

namespace QuickTools.Windows.Handlers.FolderHandlers
{
    public static class FolderHandler
    {
        public static void OpenFolder(UIntPtr window, UIntPtr event_type, IntPtr element, UIntPtr event_number, UIntPtr bind_id)
        {
            IntPtr dataPtr = InterfaceMethods.webui_interface_get_string_at(window, event_number, UIntPtr.Zero);
            string folderPath = MarshalHelper.PtrToString(dataPtr);
            
            Task.Run(() =>
            {
                FolderHelper.OpenFolder(folderPath);
            });
        }
    }
}