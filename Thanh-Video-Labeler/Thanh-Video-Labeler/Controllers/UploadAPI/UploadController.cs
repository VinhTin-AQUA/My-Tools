using Aspose.Cells;
using Microsoft.AspNetCore.Mvc;
using Thanh_Video_Labeler.Constants;
using Thanh_Video_Labeler.Controllers.Common;
using Thanh_Video_Labeler.Controllers.UploadAPI.Payload;
using Thanh_Video_Labeler.Enums;
using Thanh_Video_Labeler.Models;
using Thanh_Video_Labeler.Repositories.ConfigRepository;
using Thanh_Video_Labeler.Repositories.SheetRepository;
using Thanh_Video_Labeler.Repositories.VideoInfoRepository;
using Thanh_Video_Labeler.Services;

namespace Thanh_Video_Labeler.Controllers.UploadAPI
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly FileService fileService;
        private readonly ConfigService configService;
        private readonly IConfigCommandRepository configCommandRepository;
        private readonly ISheetCommandRepository sheetCommandRepository;
        private readonly ISheetQueryRepository sheetQueryRepository;
        private readonly IVideoInfoQueryRepository videoInfoQueryRepository;
        private readonly IVideoInfoCommandRepository videoInfoCommandRepository;
        private readonly IWebHostEnvironment env;

        public UploadController(FileService fileService, 
            ConfigService configService,
            IConfigCommandRepository  configCommandRepository,
            ISheetCommandRepository sheetCommandRepository,
            ISheetQueryRepository sheetQueryRepository,
            IVideoInfoQueryRepository videoInfoQueryRepository,
            IVideoInfoCommandRepository videoInfoCommandRepository,
            IWebHostEnvironment env)
        {
            this.fileService = fileService;
            this.configService = configService;
            this.configCommandRepository = configCommandRepository;
            this.sheetCommandRepository = sheetCommandRepository;
            this.sheetQueryRepository = sheetQueryRepository;
            this.videoInfoQueryRepository = videoInfoQueryRepository;
            this.videoInfoCommandRepository = videoInfoCommandRepository;
            this.env = env;
        }

        [HttpPost]
        public async Task<IActionResult> UploadExcel([FromForm] UploadFile uploadFile)
        {
            if (uploadFile.File == null || uploadFile.File.Length == 0)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Data = null,
                    Message = "File is invalid."
                });
            }
            
            string folderpath = Path.Combine(env.WebRootPath, FolderConstants.ExcelFolder);
            if (!uploadFile.IsAccepted && System.IO.File.Exists(Path.Combine(folderpath, uploadFile.File.FileName)))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Data = new
                    {
                        IsAccepted = !uploadFile.IsAccepted,
                    },
                    Message = "The file already exists in the system. If you continue to upload, the old data will be lost."
                });
            }
            
            _ = await fileService.SaveFile(uploadFile.File, folderpath);

            /* xóa sheet */
            var oldSheets = await sheetQueryRepository.GetAllAsync();
            await sheetCommandRepository.DeleteRangeAsync(oldSheets.ToList());

            /* xóa video */
            var oldVideos = await videoInfoQueryRepository.GetAllAsync();
            await videoInfoCommandRepository.DeleteRangeAsync(oldVideos.ToList());

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

            /* thêm sheet mới */
            using Workbook workbook = new Workbook(uploadFile.File.OpenReadStream());
            var sheets = workbook.Worksheets.Select(x => new Sheet() { SheetCode = x.CodeName, SheetName = x.Name, SheetStatus = ESheetStatus.Pending }).ToList();
            await sheetCommandRepository.AddRangeAsync(sheets);

            /* update config */
            ConfigService.Config.ExceFileName = uploadFile.File.FileName;
            ConfigService.Config.SheetCode = sheets[0].SheetCode;
            ConfigService.Config.SheetName = sheets[0].SheetName;
            await configCommandRepository.UpdateAsync(ConfigService.Config);

            return Ok(new ApiResponse<object>
            {
                Data = null,
                Message = "Upload Successfully."
            });
        }
    }
}