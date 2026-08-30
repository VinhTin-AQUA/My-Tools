using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using QuickTools.Services.Models.Iloveimg;

namespace QuickTools.Services.Iloveimg
{
    public class IloveImgUpscaleImageService
    {
        private static readonly string token =
            "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiIiLCJhdWQiOiIiLCJpYXQiOjE1MjMzNjQ4MjQsIm5iZiI6MTUyMzM2NDgyNCwianRpIjoicHJvamVjdF9wdWJsaWNfYzkwNWRkMWMwMWU5ZmQ3NzY5ODNjYTQwZDBhOWQyZjNfT1Vzd2EwODA0MGI4ZDJjN2NhM2NjZGE2MGQ2MTBhMmRkY2U3NyJ9.qvHSXgCJgqpC4gd6-paUlDLFmg0o2DsOvb1EUYPYx_E";

        private static readonly string userAgent =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36 Edg/131.0.0.0";

        private static readonly string[] servers =
        [
            "api1g", "api2g", "api3g", "api7g", "api8g", "api9g", "api10g", "api11g", "api12g", "api13g", "api14g",
            "api15g", "api16g", "api17g", "api18g", "api19g", "api20g", "api1g", "api1g", "api1g", "api2g", "api2g",
            "api2g", "api3g", "api3g", "api3g", "api11g", "api11g", "api11g"
        ];

        public static async Task<(List<IloveImgUpscaleUploadResponse>, string, string)> UploadServer(IloveImgUpscaleImageRequest request)
        {
            Random random = new();
            var server = servers[random.Next(0, servers.Length)];
            var taskId = await GetTaskId();
            List<IloveImgUpscaleUploadResponse> uploadResponses = [];
            var uploadUrl = $"https://{server}.iloveimg.com/v1/upload";
            
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(5); // Tăng thời gian chờ lên 5 phút
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Add("User-Agent", userAgent);

                foreach (var upscaleImageRequestItem in request.UpscaleImageRequestItems)
                {
                    var fileContent = new ByteArrayContent(File.ReadAllBytes(upscaleImageRequestItem.LocalPath)); // Thay bằng đường dẫn đến tệp bạn muốn tải lên
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg"); // Hoặc kiểu tệp của bạn (ví dụ: image/jpeg)
                    var bytes = await fileContent.ReadAsByteArrayAsync();

                    var form = new MultipartFormDataContent
                    {
                        { new ByteArrayContent(bytes), "file", Path.GetFileName(upscaleImageRequestItem.LocalPath) },
                        { new StringContent(Path.GetFileName(upscaleImageRequestItem.LocalPath)), "name" },
                        { new StringContent("0"), "chunk" },
                        { new StringContent("1"), "chunks" },
                        { new StringContent(taskId!), "task" },
                        { new StringContent("1"), "preview" },
                        { new StringContent("0"), "pdfinfo" },
                        { new StringContent("0"), "pdfforms" },
                        { new StringContent("0"), "pdfresetforms" },
                        { new StringContent("web.0"), "v" }
                    };

                    var response = await client.PostAsync(uploadUrl, form);
                    response.EnsureSuccessStatusCode(); // Kiểm tra phản hồi có thành công không
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var uploadResponse = JsonSerializer.Deserialize<IloveImgUpscaleUploadResponse>(responseContent);
                    fileContent.Dispose();

                    if (uploadResponse != null)
                    {
                        uploadResponse.Id = upscaleImageRequestItem.Id;
                        uploadResponse.Name = upscaleImageRequestItem.Name;
                        uploadResponses.Add(uploadResponse);
                    }
                    await Task.Delay(600);
                }
            }

            return (uploadResponses, server, taskId ?? "");
        }

        public static async Task<byte[]> Upscale(
            IloveImgUpscaleUploadResponse iloveImgUpscaleUploadResponse, 
            string server, 
            string taskId, 
            string scale
        )
        {
            var scaleUrl = $"https://{server}.iloveimg.com/v1/upscale";
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(5); // Tăng thời gian chờ lên 5 phút
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                client.DefaultRequestHeaders.Add("User-Agent", userAgent);

                var form = new MultipartFormDataContent
                {
                    { new StringContent(taskId), "task" },
                    { new StringContent(iloveImgUpscaleUploadResponse.server_filename), "server_filename" },
                    { new StringContent(scale), "scale" }
                };

                var response = await client.PostAsync(scaleUrl, form);
                response.EnsureSuccessStatusCode();
                var imageBytes = await response.Content.ReadAsByteArrayAsync();

                await Task.Delay(1200);
                return imageBytes;
            }
        }

        private static async Task<string?> GetTaskId()
        {
            var url = "https://www.iloveimg.com/upscale-image";
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(5);
                    client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                    var responseTaskId = await client.GetAsync(url);
                    responseTaskId.EnsureSuccessStatusCode();
                    var contentTaskId = await responseTaskId.Content.ReadAsStringAsync();

                    var pattern = @"ilovepdfConfig\.taskId\s*=\s*'([^']*)'";
                    var match = Regex.Match(contentTaskId, pattern);

                    if (match.Success)
                    {
                        var task_Id = match.Groups[1].Value;

                        return task_Id;
                    }
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public class IloveImgUpscaleUploadResponse
    {
        public string Id { get; set; } = string.Empty;  
        public string Name { get; set; } = string.Empty;  
        public string server_filename { get; set; } = string.Empty;
    }
}