using Thanh_Video_Labeler.Enums;

namespace Thanh_Video_Labeler.Models
{
public class VideoInfo : Entity
    {
        public string TransID { get; set; } = string.Empty;
        public string TransNB { get; set; } = string.Empty;
        public string TotalAmount { get; set; } = string.Empty;
        public string DVRStart { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string StartShift { get; set; } = string.Empty;
        public string EndShift { get; set; } = string.Empty;
        public string EmployeeID { get; set; } = string.Empty;
        public string RegisterName { get; set; } = string.Empty;
        public string LoyaltyCard { get; set; } = string.Empty;
        public string SiteName { get; set; } = string.Empty;
        public string SiteEmployee { get; set; } = string.Empty;
        public string ExceptionAmount { get; set; } = string.Empty;
        public string TOperatorID { get; set; } = string.Empty;
        public string TPACID { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string RegionName { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public string ExceptionType { get; set; } = string.Empty;
        public string Payment { get; set; } = string.Empty;
        public string CardNo { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string TransDate { get; set; } = string.Empty;
        public string Assign { get; set; } = string.Empty;
        public string HighRisk { get; set; } = string.Empty;
        public string PreTransID { get; set; } = string.Empty;
        public string PreTransDate { get; set; } = string.Empty;
        public string PreTotalAmount { get; set; } = string.Empty;
        public string HasRedeemLater { get; set; } = string.Empty;
        public string YPredProba { get; set; } = string.Empty;
        public string Bucket { get; set; } = string.Empty;
        public string BucketSmall { get; set; } = string.Empty;
        public string TransDateWeekdayMask { get; set; } = string.Empty;
        public string TransDateFormatted { get; set; } = string.Empty;

        public VideoStatus VideoStatus { get; set; }
    }
}