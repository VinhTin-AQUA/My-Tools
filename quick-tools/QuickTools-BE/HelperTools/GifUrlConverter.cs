using System.Text.RegularExpressions;
using System.Web;

namespace HelperTools
{
    public class GifUrlConverter
    {
        /// <summary>
        ///     Bước 1: Từ URL Facebook proxy -> URL Giphy gốc
        ///     Lấy tham số "url" và decode URL-encoding
        /// </summary>
        public static string ExtractOriginalUrl(string facebookUrl)
        {
            if (string.IsNullOrWhiteSpace(facebookUrl))
                throw new ArgumentException("URL không được rỗng", nameof(facebookUrl));

            var uri = new Uri(facebookUrl);
            var query = HttpUtility.ParseQueryString(uri.Query);
            // Hoặc với .NET Core: QueryHelpers.ParseQuery(uri.Query)

            var rawUrl = query["url"];
            if (string.IsNullOrEmpty(rawUrl))
                throw new InvalidOperationException("Không tìm thấy tham số 'url' trong URL.");

            // HttpUtility.ParseQueryString đã tự decode -> rawUrl là URL gốc
            return rawUrl;
        }

        /// <summary>
        ///     Bước 2: Rút gọn URL Giphy dài -> URL Giphy ngắn
        ///     Ví dụ: https://media3.giphy.com/media/v1.Y2lk.../NzSUEgbTWB7TW/200.gif
        ///     -> https://media3.giphy.com/media/NzSUEgbTWB7TW/200.gif
        /// </summary>
        public static string ShortenGiphyUrl(string giphyUrl)
        {
            if (string.IsNullOrWhiteSpace(giphyUrl))
                throw new ArgumentException("URL không được rỗng", nameof(giphyUrl));

            var uri = new Uri(giphyUrl);

            // Tách các segment của path: /media/v1.Y2lk.../NzSUEgbTWB7TW/200.gif
            var segments = uri.AbsolutePath.Trim('/').Split('/');

            // Tìm segment chứa ID (thường là segment áp cuối, trước tên file)
            // Bỏ qua các segment bắt đầu bằng "v1." hoặc các segment version/metadata
            string mediaId = null;
            for (var i = segments.Length - 2; i >= 0; i--)
            {
                var seg = segments[i];
                if (seg.StartsWith("v1.", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (seg.StartsWith("v", StringComparison.OrdinalIgnoreCase) &&
                    seg.Length > 1 && char.IsDigit(seg[1]))
                    continue;

                mediaId = seg;
                break;
            }

            if (string.IsNullOrEmpty(mediaId))
                throw new InvalidOperationException("Không tìm thấy ID media trong URL Giphy.");

            var fileName = segments[segments.Length - 1]; // "200.gif"

            // Ghép lại URL ngắn
            var baseUrl = $"https://i.giphy.com";
            return $"{baseUrl}/{mediaId}.gif";
        }

        public static async Task<string?> GetGifName(string url)
        {
            using var client = new HttpClient();

            using var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Giống curl
            request.Headers.TryAddWithoutValidation(
                "Accept",
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8");

            request.Headers.TryAddWithoutValidation(
                "Accept-Language",
                "en-US,en;q=0.9");

            request.Headers.TryAddWithoutValidation(
                "Cache-Control",
                "max-age=0");

            request.Headers.TryAddWithoutValidation(
                "Priority",
                "u=0, i");

            request.Headers.TryAddWithoutValidation(
                "Sec-CH-UA",
                "\"Chromium\";v=\"152\", \"Not?A_Brand\";v=\"24\", \"Brave\";v=\"152\"");

            request.Headers.TryAddWithoutValidation(
                "Sec-CH-UA-Mobile",
                "?0");

            request.Headers.TryAddWithoutValidation(
                "Sec-CH-UA-Platform",
                "\"Linux\"");

            request.Headers.TryAddWithoutValidation(
                "Sec-Fetch-Dest",
                "document");

            request.Headers.TryAddWithoutValidation(
                "Sec-Fetch-Mode",
                "navigate");

            request.Headers.TryAddWithoutValidation(
                "Sec-Fetch-Site",
                "same-origin");

            request.Headers.TryAddWithoutValidation(
                "Sec-Fetch-User",
                "?1");

            request.Headers.TryAddWithoutValidation(
                "Sec-GPC",
                "1");

            request.Headers.TryAddWithoutValidation(
                "Upgrade-Insecure-Requests",
                "1");

            request.Headers.TryAddWithoutValidation(
                "User-Agent",
                "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 " +
                "(KHTML, like Gecko) Chrome/152.0.0.0 Safari/537.36");

            using var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync();
            int t = html.Length;
            
            // Lấy <title>
            var match = Regex.Match(
                html,
                @"<title[^>]*>(.*?)</title>",
                RegexOptions.Singleline | RegexOptions.IgnoreCase);

            if (!match.Success)
                return null;

            var title = HttpUtility.HtmlDecode(match.Groups[1].Value);

            const string suffix = " - Find & Share on GIPHY";

            var idx = title.IndexOf(
                suffix,
                StringComparison.OrdinalIgnoreCase);

            if (idx >= 0)
                title = title.Substring(0, idx);

            // Delay 3-5 giây
            await Task.Delay(Random.Shared.Next(2000, 4001));

            return title.Trim();
        }
    }
}