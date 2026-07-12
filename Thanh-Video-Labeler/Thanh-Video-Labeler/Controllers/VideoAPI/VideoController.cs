using Aspose.Cells;
using Microsoft.AspNetCore.Mvc;
using Thanh_Video_Labeler.Constants;
using Thanh_Video_Labeler.Controllers.Common;
using Thanh_Video_Labeler.Controllers.VideoAPI.Payload;
using Thanh_Video_Labeler.Enums;
using Thanh_Video_Labeler.Hubs;
using Thanh_Video_Labeler.Models;
using Thanh_Video_Labeler.Repositories.ConfigRepository;
using Thanh_Video_Labeler.Repositories.Models;
using Thanh_Video_Labeler.Repositories.SheetRepository;
using Thanh_Video_Labeler.Repositories.VideoInfoRepository;
using Thanh_Video_Labeler.Services;

namespace Thanh_Video_Labeler.Controllers.VideoAPI
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly ConfigService configService;
        private readonly VideoExcelService videoExcelService;
        private readonly IVideoInfoQueryRepository videoInfoQueryRepository;
        private readonly IVideoInfoCommandRepository videoInfoCommandRepository;
        private readonly IConfigCommandRepository configCommandRepository;
        private readonly ISheetQueryRepository sheetQueryRepository;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IWebHostEnvironment env;

        private static readonly SemaphoreSlim _semaphore = new(1, 1);
        private static volatile bool _isDownloading;
        private static volatile bool _cancelRequested;
        private static CancellationTokenSource cts = new();

        public VideoController(
            ConfigService configService,
            VideoExcelService videoExcelService,
            IVideoInfoQueryRepository videoInfoQueryRepository,
            IVideoInfoCommandRepository videoInfoCommandRepository,
            IConfigCommandRepository configCommandRepository,
            ISheetQueryRepository sheetQueryRepository,
            IServiceScopeFactory scopeFactory,
            IWebHostEnvironment env)
        {
            this.configService = configService;
            this.videoExcelService = videoExcelService;
            this.videoInfoQueryRepository = videoInfoQueryRepository;
            this.videoInfoCommandRepository = videoInfoCommandRepository;
            this.configCommandRepository = configCommandRepository;
            this.sheetQueryRepository = sheetQueryRepository;
            this.scopeFactory = scopeFactory;
            this.env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetRemainingVideos()
        {
            QueryOptionsBuilder<VideoInfo> pendingVideoQueryBuilder = new();
            pendingVideoQueryBuilder.Where(x => x.VideoStatus == VideoStatus.Pending);
            var pendingVideos = await videoInfoQueryRepository.FilterAsync(pendingVideoQueryBuilder.Build());

            QueryOptionsBuilder<VideoInfo> downloadedVideoQueryBuilder = new();
            downloadedVideoQueryBuilder.Where(x => x.VideoStatus == VideoStatus.Downloaded);
            var downloadedVideos = await videoInfoQueryRepository.FilterAsync(downloadedVideoQueryBuilder.Build());

            QueryOptionsBuilder<VideoInfo> errorLinkVideoQueryBuilder = new();
            errorLinkVideoQueryBuilder.Where(x => x.VideoStatus == VideoStatus.ErrorLink);
            var errorLinkVideos = await videoInfoQueryRepository.FilterAsync(errorLinkVideoQueryBuilder.Build());

            var sheets = await sheetQueryRepository.GetAllAsync();

            return Ok(new ApiResponse<object>()
            {
                Data = new
                {
                    SampleVideos = pendingVideos.Take(5).ToList(),
                    PendingTotalVideos = pendingVideos.Count,
                    DownloadedTotalVideos = downloadedVideos.Count,
                    ErrorLinkTotalVideos = errorLinkVideos.Count,
                    SelectedSheet = new { ConfigService.Config.SheetName, ConfigService.Config.SheetCode },
                    Sheets = sheets.ToList()
                },
                Message = ""
            });
        }

        [HttpPost]
        public async Task<IActionResult> InitDownloadVideo(InitDownloadRequest downloadRequest)
        {
            if (string.IsNullOrEmpty(ConfigService.Config.ExceFileName))
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Data = null,
                    Message = "File does not exist. Please upload file."
                });
            }

            // Load file Excel
            string filePath = Path.Combine(env.WebRootPath, FolderConstants.ExcelFolder,
                ConfigService.Config.ExceFileName);
            if (!System.IO.File.Exists(filePath))
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Data = null,
                    Message = "File does not exist. Please upload file."
                });
            }

            var exists = await videoInfoQueryRepository.ExistsAsync();
            if (exists)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Data = null,
                    Message = "Please complete the labeling process and export the excel file with removing data option."
                });
            }

            using Workbook workbook = new Workbook(filePath);
            using Worksheet? sheet = workbook.Worksheets.Where(x => x.CodeName == downloadRequest.SheetCode).FirstOrDefault();
            if (sheet == null)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Data = null,
                    Message = "Sheet not found."
                });
            }

            // update config
            ConfigService.Config.SheetCode = downloadRequest.SheetCode;
            ConfigService.Config.SheetName = sheet.Name;
            await configCommandRepository.UpdateAsync(ConfigService.Config);

            // thêm video mới
            var listVideo = videoExcelService.ReadDataFromSheet(sheet);
            if (listVideo.Count == 0)
            {
                return BadRequest(new ApiResponse<List<VideoInfo>>()
                {
                    Data = listVideo.ToList(),
                    Message = "Out of data. Please select other sheet and init again."
                });
            }
            var listNewVideo = await videoInfoCommandRepository.AddRangeAsync(listVideo);
            return Ok(new ApiResponse<object>()
            {
                Data = new
                {
                    SampleVideos = listNewVideo.Take(5).ToList(),
                    PendingTotalVideos = listNewVideo.Count,
                    SelectedSheet = new { ConfigService.Config.SheetName, ConfigService.Config.SheetCode },
                },
                Message = "Init Successfully."
            });
        }

        [HttpPost]
        public async Task<IActionResult> StartDownload(DownloadVideoRequest downloadVideoRequest)
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
            _cancelRequested = false;
            var token = cts.Token;

            try
            {
                var queryOptionBuilder = new QueryOptionsBuilder<VideoInfo>();
                queryOptionBuilder.Where(x => x.VideoStatus == VideoStatus.Pending);
                var listVideo = await videoInfoQueryRepository.FilterAsync(queryOptionBuilder.Build());
                int total = listVideo.Count;

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

                int maxConcurrentDownloads = 2; // Số video tải đồng thời

                _ = Task.Run(async () =>
                {
                    using var scope = scopeFactory.CreateScope();
                    var _VideoService = scope.ServiceProvider.GetRequiredService<VideoService>();
                    var _videoInfoCommandRepository = scope.ServiceProvider.GetRequiredService<IVideoInfoCommandRepository>();
                    var _videoDownloadHubService = scope.ServiceProvider.GetRequiredService<VideoDownloadHubService>();

                    var semaphoreSlim = new SemaphoreSlim(maxConcurrentDownloads);
                    var downloadTasks = new List<Task>();
                    int totalSuccess = 0;
                    int totalFailed = 0;

                    try
                    {
                        for (int i = 0; i < total; i++)
                        {
                            if (_cancelRequested || token.IsCancellationRequested)
                                break;

                            await semaphoreSlim.WaitAsync(token);

                            int index = i; // Local copy for closure
                            var task = Task.Run(async () =>
                            {
                                try
                                {
                                    var video = listVideo[index];
                                    bool check = await _VideoService.DownloadVideo(video.Link, video.TransID, token);

                                    if (!check)
                                    {
                                        video.VideoStatus = VideoStatus.ErrorLink;
                                        Interlocked.Increment(ref totalFailed);
                                        await _videoDownloadHubService.RecieveTotalVideo(new ResultDownloadVideo()
                                        {
                                            IsSuccess = false,
                                            TotalDownloadsSuccess = 0,
                                            TotalDownloadsFailed = 1,
                                            Link = video.Link,
                                            TransId = video.TransID,
                                        });
                                    }
                                    else
                                    {
                                        video.VideoStatus = VideoStatus.Downloaded;
                                        Interlocked.Increment(ref totalSuccess);
                                        await _videoDownloadHubService.RecieveTotalVideo(new ResultDownloadVideo()
                                        {
                                            IsSuccess = true,
                                            TotalDownloadsSuccess = 1,
                                            TotalDownloadsFailed = 0,
                                            Link = video.Link,
                                            TransId = video.TransID,
                                        });
                                        await _VideoService.SaveFileVideoInfo(video);
                                    }

                                    await _videoInfoCommandRepository.UpdateAsync(video);
                                    if (totalSuccess + totalFailed >= downloadVideoRequest.TotalToDownload)
                                    {
                                        cts.Cancel();
                                    }
                                }
                                catch (Exception ex)
                                {
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
                        await _videoDownloadHubService.SendDownloadFinish(true);
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

            return Ok(new ApiResponse<object>
            {
                Message = "Stop downloading."
            });
        }

        [HttpGet]
        public async Task<IActionResult> Filter(string? transIdOrPayment, bool? label, bool? noLabel, int page = 1,
            int pageSize = 20)
        {
            QueryOptionsBuilder<VideoInfo> queryOptionBuilder = new();
            queryOptionBuilder.Skip(pageSize * (page - 1));
            queryOptionBuilder.Take(pageSize);
            if (!string.IsNullOrEmpty(transIdOrPayment))
            {
                queryOptionBuilder.Where(v => v.TransID.ToLower().Contains(transIdOrPayment.ToLower()) ||
                                              v.Payment.ToLower().Contains(transIdOrPayment.ToLower()));
            }

            if (label != null)
            {
                if (label.Value)
                {
                    queryOptionBuilder.Where(x => string.IsNullOrEmpty(x.Label) == false);
                }
            }

            if (noLabel != null)
            {
                if (noLabel.Value)
                {
                    queryOptionBuilder.Where(x => string.IsNullOrEmpty(x.Label) == true);
                }
            }
            var list = await videoInfoQueryRepository.FilterAsync(queryOptionBuilder.Build());
            return Ok(
                new ApiResponse<List<VideoInfo>>()
                {
                    Data = list,
                    Message = "",
                });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateVideo(UpdateVideo model)
        {
            var video = await videoInfoQueryRepository.GetByIdAsync(model.Id);
            if (video == null)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Message = "Video not found."
                });
            }
            video.Label = model.Label;
            await videoInfoCommandRepository.UpdateAsync(video);

            return Ok(new ApiResponse<object>()
            {
                Message = "Update Successfully."
            });
        }

        [HttpPost]
        public async Task<IActionResult> ExportExcel(ExportExcel exportExcel)
        {
            var data = await videoInfoQueryRepository.GetAllAsync();
            var byteData = videoExcelService.ExportExcel(data.ToList());

            if (exportExcel.ClearAllData)
            {
                /* xóa file video trong folder */
                string folderPath = Path.Combine(env.WebRootPath, FolderConstants.VideoFolder);
                if (Directory.Exists(folderPath))
                {
                    string[] files = Directory.GetFiles(folderPath);

                    foreach (var file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                }
                await videoInfoCommandRepository.DeleteRangeAsync(data.ToList());
            }
            return File(byteData,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Export.xlsx");
        }
    }
}