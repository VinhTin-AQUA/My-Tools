using QuickTools.Windows.Modules.WebUI.Methods;

namespace QuickTools.Windows.Modules.WebUI
{
    public static class JavaScriptExecution
    {
        public static void WebuiRun(UIntPtr window, string script)
        {
            JavaScriptExecutionMethods.webui_run(window, script);
        }
    }
}