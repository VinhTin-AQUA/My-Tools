using CaesiumSharp;

namespace QuickTools.Services.Libcaesium
{
    public class LibcaesiumService
    {
        public static string CompressImages(uint quality, string input, string outputFolder)
        {
            string output = "";
            using (var compressor = new CaesiumCompressor())
            {
                // var parameters = compressor.GetDefaultParameters();
                var parameters = new CCSParameters()
                {
                    KeepMetadata = false,
                    JpegQuality = quality,
                    PngQuality = quality,
                    GifQuality = quality,
                    WebpQuality = quality,
                };

                string fileName = Path.GetFileName(input);
                string folderToSaveImages =  Path.Combine(outputFolder, "Compressed");
                
                if (!Directory.Exists(folderToSaveImages))
                {
                    Directory.CreateDirectory(folderToSaveImages);
                }
                output = Path.Combine(folderToSaveImages, fileName);

                if (compressor.CompressFile(input, output, parameters, out string? error))
                {
                    Console.WriteLine("Compression successful!");
                }
                else
                {
                    Console.WriteLine($"Compression failed: {error}");
                }
            }

            return output;
        }

        public static byte[]? CompressInMemory(uint quality, byte[] inputData)
        {
            using (var compressor = new CaesiumCompressor())
            {
                // var parameters = compressor.GetDefaultParameters();
                var parameters = new CCSParameters()
                {
                    KeepMetadata = false,
                    JpegQuality = quality,
                    PngQuality = quality,
                    GifQuality = quality,
                    WebpQuality = quality,
                };
          
                if (compressor.CompressInMemory(inputData, parameters, out byte[]? outdata, out string? error))
                {
                    Console.WriteLine("Compression successful!");
                    return outdata;
                }
                else
                {
                    Console.WriteLine($"Compression failed: {error}");
                }
            }

            return null;
        }
    }
}