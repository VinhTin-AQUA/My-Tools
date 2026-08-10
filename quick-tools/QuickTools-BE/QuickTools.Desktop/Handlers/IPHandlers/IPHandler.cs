using QuickTools.Services.IP;
using QuickTools.Services.Models.IP;

namespace QuickTools.Windows.Handlers.IPHandlers
{
    public static class IPHandler
    {
        public static async Task<object> GetIpInfo(UIntPtr window, UIntPtr event_type, IntPtr element, UIntPtr event_number, UIntPtr bind_id)
        {
            var ip = await IPService.GetIpInformationAsync();
            return ip;
        }
    }
}