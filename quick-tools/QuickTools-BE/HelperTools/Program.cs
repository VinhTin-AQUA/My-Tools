namespace HelperTools
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // var inputFile = "/home/newtun/Desktop/My-Tools/data/icons/icon_links.txt";
            // var outputFile = "/home/newtun/Desktop/My-Tools/data/icons/temp_icon_data.txt";
            //
            // // Đọc danh sách URL, mỗi URL một dòng
            // var urls = await File.ReadAllLinesAsync(inputFile);
            // Console.WriteLine($"Tìm thấy {urls.Length} link.");
            //
            // for (var i = 0; i < urls.Length; i++)
            // {
            //     var url = urls[i];
            //
            //     if (string.IsNullOrWhiteSpace(url))
            //     {
            //         continue;
            //     }   
            //
            //     try
            //     {
            //         Console.WriteLine($"[{i + 1}/{urls.Length}] Đang xử lý: {url}");
            //
            //         var extractOriginalUrl = GifUrlConverter.ExtractOriginalUrl(url);
            //
            //         // Lấy gif name
            //         var gifName = await GifUrlConverter.GetGifName(extractOriginalUrl);
            //         
            //         // Lấy shortened Giphy URL
            //         var shortenGiphyUrl = GifUrlConverter.ShortenGiphyUrl(extractOriginalUrl);
            //         
            //         if (string.IsNullOrEmpty(extractOriginalUrl) 
            //             || string.IsNullOrEmpty(gifName) 
            //             || string.IsNullOrEmpty(shortenGiphyUrl)
            //         )
            //         {
            //             // throw new Exception("gifName is null");
            //             continue;
            //         }
            //
            //         var gif = $"{gifName}\n{shortenGiphyUrl}";
            //
            //         // Append ngay kết quả vào cuối file
            //         await File.AppendAllTextAsync(
            //             outputFile,
            //             gif + Environment.NewLine
            //         );
            //
            //         Console.WriteLine($"[{i + 1}/{urls.Length}] Hoàn thành: {gifName}");
            //     }
            //     catch (Exception ex)
            //     {
            //         Console.WriteLine();
            //         Console.WriteLine("========== CÓ LỖI ==========");
            //         Console.WriteLine($"Link thứ: {i + 1}");
            //         Console.WriteLine($"URL: {url}");
            //         Console.WriteLine($"Lỗi: {ex.Message}");
            //         Console.WriteLine("============================");
            //         Console.WriteLine();
            //
            //         // Dừng chương trình
            //         return;
            //     }
            // }
            //
            // Console.WriteLine();
            // Console.WriteLine("Đã xử lý xong tất cả link.");
            
            
            string inputPath = @"/home/newtun/Desktop/My-Tools/data/icons/temp_icon_data.txt";
            string outputPath = @"/home/newtun/Desktop/My-Tools/data/icons/final_icons.txt";

            string[] lines = File.ReadAllLines(inputPath);

            // Parse thành danh sách (name, url)
            var items = new List<(string Name, string Url)>();

            for (int i = 0; i < lines.Length; i += 2)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;
                if (i + 1 >= lines.Length) break;

                string name = lines[i].Trim();
                string url = lines[i + 1].Trim();

                items.Add((name, url));
            }

            // Nhóm theo URL
            var groups = items
                .GroupBy(x => x.Url)
                .ToList();

            // Lấy phần tử đầu tiên của mỗi nhóm (danh sách không trùng)
            var uniqueItems = groups
                .Select(g => g.First())
                .ToList();

            // Lấy các phần tử bị trùng (từ phần tử thứ 2 trở đi trong mỗi nhóm)
            var duplicates = groups
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Skip(1)) // bỏ phần tử đầu, giữ các phần tử trùng
                .ToList();

            // Ghi file output
            using (var writer = new StreamWriter(outputPath))
            {
                foreach (var item in uniqueItems)
                {
                    writer.WriteLine(item.Name);
                    writer.WriteLine(item.Url);
                }
            }

            // In thống kê
            Console.WriteLine($"Tổng số phần tử gốc: {items.Count}");
            Console.WriteLine($"Số phần tử sau khi loại trùng: {uniqueItems.Count}");
            Console.WriteLine($"Số phần tử bị trùng: {duplicates.Count}");
            Console.WriteLine();

            // In chi tiết các cặp bị trùng
            if (duplicates.Count > 0)
            {
                Console.WriteLine("=== DANH SÁCH CÁC CẶP BỊ TRÙNG (theo URL) ===");
                Console.WriteLine();

                int index = 1;
                foreach (var g in groups.Where(g => g.Count() > 1))
                {
                    Console.WriteLine($"[Nhóm trùng #{index}] URL: {g.Key}");
                    Console.WriteLine($"  → Giữ lại: \"{g.First().Name}\"");
                    Console.WriteLine($"  → Bị trùng ({g.Count() - 1}):");

                    foreach (var dup in g.Skip(1))
                    {
                        Console.WriteLine($"      - \"{dup.Name}\"");
                    }

                    Console.WriteLine();
                    index++;
                }
            }
            else
            {
                Console.WriteLine("Không có phần tử trùng nào.");
            }
        }
    }
}