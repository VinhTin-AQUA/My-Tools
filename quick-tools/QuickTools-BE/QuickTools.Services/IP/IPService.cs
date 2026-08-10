using System.Text.Json;
using QuickTools.Services.Models.IP;

namespace QuickTools.Services.IP
{
    public static class IPService
    {
        public static async Task<IpApiResponse> GetIpInformationAsync()
        {
            using var httpClient = new HttpClient();

            // 1. Lấy Public IP
            var ipifyResponse = await httpClient.GetAsync(
                "https://api.ipify.org/?format=json");

            ipifyResponse.EnsureSuccessStatusCode();

            var ipifyJson = await ipifyResponse.Content.ReadAsStringAsync();

            var ipInfo = JsonSerializer.Deserialize<IpifyResponse>(ipifyJson);

            if (ipInfo == null || string.IsNullOrWhiteSpace(ipInfo.Ip))
            {
                throw new Exception("Không lấy được Public IP.");
            }

            // 2. Dùng IP để lấy thông tin địa lý
            var ipApiUrl = $"http://ip-api.com/json/{ipInfo.Ip}";

            var locationResponse = await httpClient.GetAsync(ipApiUrl);

            locationResponse.EnsureSuccessStatusCode();

            var locationJson = await locationResponse.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<IpApiResponse>(locationJson);

            if (result == null)
            {
                throw new Exception("Không lấy được thông tin IP.");
            }

            // 3. Trả về thông tin
            return result;
        }
    }
}