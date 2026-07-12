using Aspose.Cells;
using Thanh_Video_Labeler.Constants;
using Thanh_Video_Labeler.Enums;
using Thanh_Video_Labeler.Models;

namespace Thanh_Video_Labeler.Services
{
    public class VideoExcelService
    {
        public void ReadExcel(IFormFile file)
        {
            using var stream = new MemoryStream();
            file.CopyTo(stream);
            stream.Position = 0;

            var workbook = new Workbook(stream);
            var worksheet = workbook.Worksheets[0];
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
                foreach (var header in VideoConstants.ExpectedHeaders)
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

            _ = MapToVideoInfos(result);
        }

        public List<VideoInfo> ReadDataFromSheet(Worksheet worksheet)
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
            for (int row = 0; row <= rowCount; row++)
            {
                var rowData = new Dictionary<string, string>();
                foreach (var header in VideoConstants.ExpectedHeaders)
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

        public byte[] ExportExcel(List<VideoInfo> data)
        {
            using Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets[0];
            sheet.Name = "Data";

            string[] headers =
            [
                "TransID", "TransNB", "Total_amount", "DVRStart",
                "Channel", "start_shift", "end_shift", "EmployeeID",
                "Register_Name", "Loyalty_card", "SiteName", "SiteEmployee",
                "exception_amount", "T_OperatorID", "T_PACID", "City", "RegionName",
                "Division", "exception_type", "Payment", "CardNo", "Desc", "link",
                "Label", "TransDate", "asign", "high_risk", "pre_TransID", "Pre_TransDate",
                "Pre_Total_amount", "has_redeem_later", "y_pred_proba", "bucket", "bucket_small",
                "TransDateWeekdayMask", "TransDate_formatted"
            ];

            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Cells[0, i].PutValue(headers[i]);
                sheet.Cells.SetColumnWidth(i, 20);
            }

            int total = data.Count;
            
            for (int i = 0; i < total; i++)
            {
                sheet.Cells[i + 1, 0].PutValue(data[i].TransID);
                sheet.Cells[i + 1, 1].PutValue(data[i].TransNB);
                sheet.Cells[i + 1, 2].PutValue(data[i].TotalAmount);
                sheet.Cells[i + 1, 3].PutValue(data[i].DVRStart);
                sheet.Cells[i + 1, 4].PutValue(data[i].Channel);
                sheet.Cells[i + 1, 5].PutValue(data[i].StartShift);
                sheet.Cells[i + 1, 6].PutValue(data[i].EndShift);
                sheet.Cells[i + 1, 7].PutValue(data[i].EmployeeID);
                sheet.Cells[i + 1, 8].PutValue(data[i].RegisterName);
                sheet.Cells[i + 1, 9].PutValue(data[i].LoyaltyCard);
                sheet.Cells[i + 1, 10].PutValue(data[i].SiteName);
                sheet.Cells[i + 1, 11].PutValue(data[i].SiteEmployee);
                sheet.Cells[i + 1, 12].PutValue(data[i].ExceptionAmount);
                sheet.Cells[i + 1, 13].PutValue(data[i].TOperatorID);
                sheet.Cells[i + 1, 14].PutValue(data[i].TPACID);
                sheet.Cells[i + 1, 15].PutValue(data[i].City);
                sheet.Cells[i + 1, 16].PutValue(data[i].RegionName);
                sheet.Cells[i + 1, 17].PutValue(data[i].Division);
                sheet.Cells[i + 1, 18].PutValue(data[i].ExceptionType);
                sheet.Cells[i + 1, 19].PutValue(data[i].Payment);
                sheet.Cells[i + 1, 20].PutValue(data[i].CardNo);
                sheet.Cells[i + 1, 21].PutValue(data[i].Desc);
                sheet.Cells[i + 1, 22].PutValue(data[i].Link);
                sheet.Cells[i + 1, 23].PutValue(data[i].Label);
                sheet.Cells[i + 1, 24].PutValue(data[i].TransDate);
                sheet.Cells[i + 1, 25].PutValue(data[i].Assign);
                sheet.Cells[i + 1, 26].PutValue(data[i].HighRisk);
                sheet.Cells[i + 1, 27].PutValue(data[i].PreTransID);
                sheet.Cells[i + 1, 28].PutValue(data[i].PreTransDate);
                sheet.Cells[i + 1, 29].PutValue(data[i].PreTotalAmount);
                sheet.Cells[i + 1, 30].PutValue(data[i].HasRedeemLater);
                sheet.Cells[i + 1, 31].PutValue(data[i].YPredProba);
                sheet.Cells[i + 1, 32].PutValue(data[i].Bucket);
                sheet.Cells[i + 1, 33].PutValue(data[i].BucketSmall);
                sheet.Cells[i + 1, 34].PutValue(data[i].TransDateWeekdayMask);
                sheet.Cells[i + 1, 35].PutValue(data[i].TransDateFormatted);
            }

            // Ghi ra stream
            using var ms = new MemoryStream();
            workbook.Save(ms, SaveFormat.Xlsx);
            ms.Position = 0;

            return ms.ToArray();
        }
        
        private List<VideoInfo> MapToVideoInfos(List<Dictionary<string, string>> rowData)
        {
            List<VideoInfo> r = [];
            foreach (var row in rowData)
            {
                VideoInfo info = new()
                {
                    TransID = row["TransID"],
                    TransNB = row["TransNB"],
                    TotalAmount = row["Total_amount"],
                    DVRStart = row["DVRStart"],
                    Channel = row["Channel"],
                    StartShift = row["start_shift"],
                    EndShift = row["end_shift"],
                    EmployeeID = row["EmployeeID"],
                    RegisterName = row["Register_Name"],
                    LoyaltyCard = row["Loyalty_card"],
                    SiteName = row["SiteName"],
                    SiteEmployee = row["SiteEmployee"],
                    ExceptionAmount = row["exception_amount"],
                    TOperatorID = row["T_OperatorID"],
                    TPACID = row["T_PACID"],
                    City = row["City"],
                    RegionName = row["RegionName"],
                    Division = row["Division"],
                    ExceptionType = row["exception_type"],
                    Payment = row["Payment"],
                    CardNo = row["CardNo"],
                    Desc = row["Desc"],
                    Link = row["link"],
                    Label = row["Label"],
                    TransDate = row["TransDate"],
                    Assign = row["Assign"],
                    HighRisk = row["high_risk"],
                    PreTransID = row["pre_TransID"],
                    PreTransDate = row["Pre_TransDate"],
                    PreTotalAmount = row["Pre_Total_amount"],
                    HasRedeemLater = row["has_redeem_later"],
                    YPredProba = row["y_pred_proba"],
                    Bucket = row["bucket"],
                    BucketSmall = row["bucket_small"],
                    TransDateWeekdayMask = row["TransDate_weekday_mask"],
                    TransDateFormatted = row["TransDate_formatted"],
                    VideoStatus = VideoStatus.Pending
                };
                r.Add(info);
            }
            return r;
        }
    }
}