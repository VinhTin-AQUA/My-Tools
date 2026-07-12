using Aspose.Cells;
using Thanh_Video_Labeler.Constants;
using Thanh_Video_Labeler.Enums;
using Thanh_Video_Labeler.Models;
using Thanh_Video_Labeler.Repositories.VideoAwsConfigRepository;

namespace Thanh_Video_Labeler.Services
{
    public class VideoAwsService
    {
        public async Task Init(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var configQueryRepository = scope.ServiceProvider.GetRequiredService<IVideoAwsConfigQueryRepository>();
            var configCommandRepository = scope.ServiceProvider.GetRequiredService<IVideoAwsConfigCommandRepository>();
            var existing = await configQueryRepository.GetByIdAsync(1);
            if (existing == null)
            {
                var defaultSetting = new VideoAwsConfig()
                {
                    Id = 1,
                    FileName = "",
                    TotalVideoAws = 0, 
                    TotalVideoAwsDownloaded = 0
                };

                await configCommandRepository.AddAsync(defaultSetting);
                // Config = defaultSetting;
            }
            else
            {
                // Config = existing;
            }
        }
        
        public List<VideoAwsInfo> ReadDataFromSheet(Worksheet worksheet)
        {
            var cells = worksheet.Cells;
            int rowCount = cells.MaxDataRow;
            int colCount = cells.MaxDataColumn;

            // Tạo dictionary map header của file Excel → index
            var excelHeaderMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int col = 0; col <= colCount; col++)
            {
                string header = cells[0, col].StringValue.Trim();
                if (!string.IsNullOrWhiteSpace(header))
                    excelHeaderMap[header] = col;
            }

            var result = new List<Dictionary<string, string>>();
            for (int row = 1; row <= rowCount; row++)
            {
                var rowData = new Dictionary<string, string>();
                foreach (var header in VideoConstants.ExpectedExcelAwsHeaders)
                {
                    if (excelHeaderMap.ContainsKey(header))
                    {
                        int colIndex = excelHeaderMap[header];
                        rowData[header] = cells[row, colIndex].StringValue?.Trim() ?? "";
                    }
                    else
                    {
                        // Header không tồn tại trong Excel, gán giá trị rỗng
                        rowData[header] = "";
                    }
                }
                result.Add(rowData);
            }

            var r = MapToVideoInfos(result);
            return r;
        }
        
        
        private List<VideoAwsInfo> MapToVideoInfos(List<Dictionary<string, string>> rowData)
        {
            List<VideoAwsInfo> r = [];
            foreach (var row in rowData)
            {
                VideoAwsInfo info = new()
                {
                    Case = row["Case"],
                    StoreLocation = row["Store Location"],
                    ServerID = row["ServerID"],
                    RequestedBy = row["Requested By"],
                    RequestedDate = row["Requested Date"],
                    DateOfIncident = row["Date Of Incident"],
                    SessionName = row["Session Name"],
                    ChannelName = row["Channel Name"],
                    BackupTime = row["Backup Time"],
                    Size = row["Size"],
                    Status = row["Status"],
                    Email = row["Email ( Contact info +  Receiver info)"],
                    Tester = row["Tester (manual input from tool)"],
                    Note = row["Note"],
                    AWSlink = row["AWS link"],
                    DateDownloadFile = row["Date download file"],
                    VideoStatus = VideoStatus.Pending
                };
                r.Add(info);
            }
            return r;
        }
    }
}