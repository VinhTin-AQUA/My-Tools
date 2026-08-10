using System.Text;
using QuickTools.Windows.Modules.WebUI.Methods;

namespace QuickTools.Windows.Modules.WebUI
{
    public static class WebUIScript
    {
        public static bool Script(UIntPtr window, string script, out string? result, uint timeout = 0)
        {
            const int bufferSize = 65536;
            var buffer = new StringBuilder(bufferSize);
            var success = JavaScriptExecutionMethods.webui_script(window, script, timeout, buffer, bufferSize);
            result = success ? buffer.ToString() : null;
            return success;
        }

        public static bool ScriptClient(ref webui_event_t e, string script, out string? result, uint timeout = 0)
        {
            const int bufferSize = 65536;
            var buffer = new StringBuilder(bufferSize);
            var success = JavaScriptExecutionMethods.webui_script_client(ref e, script, timeout, buffer, bufferSize);
            result = success ? buffer.ToString() : null;
            return success;
        }
    }
}