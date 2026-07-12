using System.IO.Compression;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Aspose.Cells;
using Microsoft.AspNetCore.Mvc;
using Thanh_Video_Labeler.Constants;
using Thanh_Video_Labeler.Controllers.Common;
using Thanh_Video_Labeler.Controllers.VideoAwsAPI.Payload;
using Thanh_Video_Labeler.Enums;
using Thanh_Video_Labeler.Hubs.VideoAws;
using Thanh_Video_Labeler.Models;
using Thanh_Video_Labeler.Repositories.Models;
using Thanh_Video_Labeler.Repositories.VideoAwsConfigRepository;
using Thanh_Video_Labeler.Repositories.VideoAwsInfoRepository;
using Thanh_Video_Labeler.Services;

namespace Thanh_Video_Labeler.Controllers.VideoAwsAPI
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class VideoAwsController : ControllerBase
    {
        private readonly IVideoAwsConfigCommandRepository videoAwsConfigCommandRepository;
        private readonly IVideoAwsConfigQueryRepository videoAwsConfigQueryRepository;
        private readonly IVideoAwsInfoCommandRepository videoAwsInfoCommandRepository;
        private readonly IVideoAwsInfoQueryRepository videoAwsInfoQueryRepository;
        private readonly IWebHostEnvironment env;
        private readonly FileService fileService;
        private readonly VideoAwsService videoAwsService;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly AwsConfig awsConfig;

        private static readonly SemaphoreSlim _semaphore = new(1, 1);
        private static volatile bool _isDownloading;
        private static CancellationTokenSource cts = new();
        private static string downloadFolder = "";

        public VideoAwsController(
            IVideoAwsConfigCommandRepository  videoAwsConfigCommandRepository,
            IVideoAwsConfigQueryRepository videoAwsConfigQueryRepository,
            IVideoAwsInfoCommandRepository  videoAwsInfoCommandRepository,
            IVideoAwsInfoQueryRepository videoAwsInfoQueryRepository,
            IWebHostEnvironment env,
            FileService fileService,
            VideoAwsService videoAwsService,
            IServiceScopeFactory scopeFactory,
            AwsConfig awsConfig
            )
        {
            this.videoAwsConfigCommandRepository = videoAwsConfigCommandRepository;
            this.videoAwsConfigQueryRepository = videoAwsConfigQueryRepository;
            this.videoAwsInfoCommandRepository = videoAwsInfoCommandRepository;
            this.videoAwsInfoQueryRepository = videoAwsInfoQueryRepository;
            this.env = env;
            this.fileService = fileService;
            this.videoAwsService = videoAwsService;
            this.scopeFactory = scopeFactory;
            this.awsConfig = awsConfig;
            
            string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string downloadsPath = Path.Combine(homePath, "Downloads");
            
            string today = DateTime.Now.ToString("ddMMyyyy");
            downloadFolder = Path.Combine(downloadsPath, today);
            if (!Directory.Exists(downloadFolder))
            {
                Directory.CreateDirectory(downloadFolder);
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> UploadExcelAws(UploadExcelAws uploadExcelAws)
        {
            if (uploadExcelAws.File == null || uploadExcelAws.File.Length == 0)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Data = null,
                    Message = "File is invalid."
                });
            }
            
            // xoa file cu 
            string folderPath = Path.Combine(env.WebRootPath, FolderConstants.AwsExcelFolder);
            var oldFiles = Directory.Exists(folderPath) ? Directory.GetFiles(folderPath) : [];
            foreach (var oldFile in oldFiles)
            {
                System.IO.File.Delete(oldFile);
            }
            
            using Workbook workbook = new Workbook(uploadExcelAws.File.OpenReadStream());
            using Worksheet? sheet = workbook.Worksheets[0];
            if (sheet == null)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Data = null,
                    Message = "Sheet not found."
                });
            }
            
            bool validHeader = ValidateHeaders(sheet);
            if (!validHeader)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Data = null,
                    Message =
                        $"Invalid Column. Columns is required: {string.Join(",", VideoConstants.ExpectedExcelAwsHeaders)}"
                });
            }
            
            // luu file moi
            _ = await fileService.SaveFile(uploadExcelAws.File, folderPath);
            
            // delete all data
            var allVideoAws = await videoAwsInfoQueryRepository.GetAllAsync();
            _ = await videoAwsInfoCommandRepository.DeleteRangeAsync(allVideoAws.ToList());
            
            // thêm video mới
            var listVideo = videoAwsService.ReadDataFromSheet(sheet);
            _ = await videoAwsInfoCommandRepository.AddRangeAsync(listVideo);
            
            // reset config
            var configs = await videoAwsConfigQueryRepository.GetByIdAsync(1);
            if (configs != null)
            {
                configs.FileName = uploadExcelAws.File.FileName;
                configs.TotalVideoAws = listVideo.Count;
                configs.TotalVideoAwsDownloaded = 0;
                _ = await videoAwsConfigCommandRepository.UpdateAsync(configs);
            }
            return Ok(new ApiResponse<object>()
            {
                Data = new
                {
                    uploadExcelAws.File.FileName,
                    TotalVideo = listVideo.Count,
                    TotalDownloaded = 0
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetRemaining()
        {
            if (string.IsNullOrEmpty(awsConfig.AccessKey) ||
                string.IsNullOrEmpty(awsConfig.SecretKey) ||
                awsConfig.AccessKey == "YOUR_ACCESS_KEY_ID" || 
                awsConfig.SecretKey == "YOUR_SECRET_ACCESS_KEY")
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Data = null,
                    Message = "Set the AWS key and secret"
                });
            }

            var config = await videoAwsConfigQueryRepository.GetByIdAsync(1);
            if (config == null)
            {
                return Ok(new ApiResponse<object>()
                {
                    Data = null,
                    Message = "Video Aws not found"
                });
            }
            
            QueryOptionsBuilder<VideoAwsInfo> builder = new();
            builder.Where(v => v.VideoStatus == VideoStatus.Downloaded);
            var listVideoAws = await videoAwsInfoQueryRepository.FilterAsync(builder.Build());
            
            return Ok(new ApiResponse<object>()
            {
                Data = new
                {
                    config.FileName,
                    TotalVideo = config.TotalVideoAws,
                    TotalDownloaded = listVideoAws.Count
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> DownloadVideoAws()
        {
            if (_isDownloading)
            {
                return Ok(new ApiResponse<object>
                {
                    Message = "Quá trình tải video đang thực hiện."
                });
            }

            cts = new CancellationTokenSource();
            await _semaphore.WaitAsync();
            _isDownloading = true;
            var token = cts.Token;

            try
            {
                QueryOptionsBuilder<VideoAwsInfo> builder = new();
                builder.Where(v => v.VideoStatus == VideoStatus.Pending);
                var listVideoAws = await videoAwsInfoQueryRepository.FilterAsync(builder.Build());
                int total = listVideoAws.Count;

                if (total == 0)
                {
                    _isDownloading = false;
                    cts.Cancel();
                    return Ok(new ApiResponse<object>
                    {
                        Data = new
                        {
                            Total = total,
                        },
                        Message = "All videos downloaded."
                    });
                }

                int maxConcurrentDownloads = 1; // Số video tải đồng thời
                _ = Task.Run(async () =>
                {
                    var semaphoreSlim = new SemaphoreSlim(maxConcurrentDownloads);
                    var downloadTasks = new List<Task>();
                    int totalSuccess = 0;
                    int totalFailed = 0;

                    try
                    {
                        for (int i = 0; i < total; i++)
                        {
                            if (token.IsCancellationRequested)
                            {
                                _isDownloading = false;
                                break;
                            }
                            var video = listVideoAws[i];
                            await semaphoreSlim.WaitAsync(token);
                            var task = Task.Run(async () =>
                            {
                                using var scope = scopeFactory.CreateScope();
                                var _videoInfoCommandRepository = scope.ServiceProvider.GetRequiredService<IVideoAwsInfoCommandRepository>();
                                var _videoDownloadHubService = scope.ServiceProvider.GetRequiredService<VideoAwsHubService>();
                                
                                
                                try
                                {
                                    await _videoDownloadHubService.RecieveDowloadingInfoVideo(video.AWSlink);
                                    bool check = await DownloadVideo(video.AWSlink, _videoDownloadHubService);
                
                                    if (token.IsCancellationRequested)
                                    {
                                        return;
                                    }
                                    
                                    if (!check)
                                    {
                                        await _videoDownloadHubService.RecieveErrorVideo(video.Case, video.ServerID, video.AWSlink);
                                        video.VideoStatus = VideoStatus.ErrorLink;
                                        Interlocked.Increment(ref totalFailed);
                                    }
                                    else
                                    {
                                        await _videoDownloadHubService.RecieveIncreaseSucess(1);
                                        video.VideoStatus = VideoStatus.Downloaded;
                                        Interlocked.Increment(ref totalSuccess);
                                    }
                                    await _videoInfoCommandRepository.UpdateAsync(video);
                                }
                                catch (Exception ex)
                                {
                                    await _videoDownloadHubService.RecieveErrorVideo(video.Case, video.ServerID, video.AWSlink);
                                    Console.WriteLine($"Lỗi khi tải video: {ex.Message}");
                                }
                                finally
                                {
                                    semaphoreSlim.Release();
                                }
                            }, token);

                            downloadTasks.Add(task);
                        }
                        await Task.WhenAll(downloadTasks);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi trong background download: {ex.Message}");
                    }
                    finally
                    {
                        // await _videoDownloadHubService.SendDownloadFinish(true);
                        _isDownloading = false;
                        _semaphore.Release();
                    }
                });

                return Ok(new ApiResponse<object>
                {
                    Message = "Downloading."
                });
            }
            catch (Exception ex)
            {
                _isDownloading = false;
                _semaphore.Release();
                return StatusCode(500, new ApiResponse<object> { Message = $"Lỗi: {ex.Message}" });
            }
        }
        
        [HttpPost]
        public IActionResult StopDownload()
        {
            if (!_isDownloading)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Message = "No download is in progress."
                });
            }

            cts.Cancel(); // Huỷ mọi token đang dùng
            _isDownloading = false;
            return Ok(new ApiResponse<object>
            {
                Message = "Stop downloading."
            });
        }
        
        #region ============================================ private methods ============================================

        private bool ValidateHeaders(Worksheet sheet)
        {
            var cells = sheet.Cells;
            int colCount = cells.MaxDataColumn;
            List<string> headers = [];
            
            for (int col = 0; col <= colCount; col++)
            {
                string header = cells[0, col].StringValue.Trim();
                if (!string.IsNullOrWhiteSpace(header))
                    headers.Add(header);
            }

            // Tìm các cột bị thiếu
            var missingColumns = VideoConstants.ExpectedExcelAwsHeaders
                .Where(reqCol => !headers.Contains(reqCol, StringComparer.OrdinalIgnoreCase))
                .ToList();

            if (missingColumns.Any())
            {
                return false;
            }
            return true;
        }

        private async Task<bool> DownloadVideo(string awsLink, VideoAwsHubService videoAwsHubService)
        {
            // string bucketName = "i3.wholefoods";
            // string keyName = "Incident/11671/PierrePasturel_20250719_11796.i3c.zip";
            
            var (bucketName, keyName, fileName) =  ParseS3Url(awsLink);
            var s3Client = new AmazonS3Client(awsConfig.AccessKey, awsConfig.SecretKey, RegionEndpoint.USEast1);
            try
            {
                Console.WriteLine("Đang tải file từ S3...");
                var request = new GetObjectRequest()
                {
                    BucketName = bucketName,
                    Key = keyName
                };

                string filePath = Path.Combine(downloadFolder, fileName);
                using (GetObjectResponse response = await s3Client.GetObjectAsync(request))
                {
                    byte[] buffer = new byte[10 * 1024 * 1024]; // 4MB buffer
                    long totalRead = 0;
                    int read;
                    double totalMB = response.ContentLength / (1024.0 * 1024.0);
                    
                    using Stream responseStream = response.ResponseStream;
                    using FileStream fileStream = System.IO.File.Create(filePath);
                    
                    while ((read = await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        if (_isDownloading == false)
                        {
                            break;
                        }
                        await fileStream.WriteAsync(buffer, 0, read);
                        totalRead += read;
                        // Console.WriteLine($"{(totalRead / (1024.0 * 1024.0)):F2} MB / {218746416 / (1024.0 * 1024.0):F2} MB");
                        
                        await videoAwsHubService.RecieveProgress(
                            $"{(totalRead / (1024.0 * 1024.0)):F2} MB / {totalMB:F2} MB");
                    }
                    await fileStream.FlushAsync();
                }

                if (_isDownloading == false)
                {
                    System.IO.File.Delete(filePath);
                    return false;
                }

                // Giải nén sau khi tải xong
                try
                {
                    string extractFolder = Path.Combine(downloadFolder, Path.GetFileNameWithoutExtension(fileName));
                    ZipFile.ExtractToDirectory(filePath, extractFolder, overwriteFiles: true);
                    
                    // Xóa file zip sau khi giải nén
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        Console.WriteLine("Đã xóa file ZIP gốc.");
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi khi giải nén: " + ex.Message);
                    return false;
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Lỗi S3: " + e.Message);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Lỗi: " + e.Message);
                return false;
            }
        }
        
        private static (string bucketName, string keyName, string fileName) ParseS3Url(string s3Url)
        {
            if (string.IsNullOrWhiteSpace(s3Url))
                throw new ArgumentException("S3 URL không hợp lệ", nameof(s3Url));

            const string prefix = "s3://";
            if (!s3Url.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("URL phải bắt đầu bằng 's3://'", nameof(s3Url));

            // Bỏ "s3://"
            var withoutPrefix = s3Url.Substring(prefix.Length);

            // Tìm vị trí dấu '/'
            int slashIndex = withoutPrefix.IndexOf('/');
            if (slashIndex < 0)
                throw new ArgumentException("URL không chứa keyName hợp lệ", nameof(s3Url));

            string bucketName = withoutPrefix.Substring(0, slashIndex);
            string keyName = withoutPrefix.Substring(slashIndex + 1);

            // Lấy tên file
            string fileName = Path.GetFileName(keyName);

            return (bucketName, keyName, fileName);
        }
        
        #endregion
    }
}