namespace Thanh_Video_Labeler.Constants
{
    public class VideoConstants
    {
        public static readonly string[] ExpectedHeaders =
        [
            "TransID", "TransNB", "Total_amount", "DVRStart",
            "Channel", "start_shift", "end_shift",
            "EmployeeID", "Register_Name", "Loyalty_card",
            "SiteName", "SiteEmployee", "exception_amount",
            "T_OperatorID", "T_PACID", "City", "RegionName",
            "Division", "exception_type",
            "Payment", "CardNo", "Desc", "link", "Label",
            "TransDate", "Assign", "high_risk",
            "pre_TransID", "Pre_TransDate", "Pre_Total_amount",
            "has_redeem_later", "y_pred_proba",
            "bucket", "bucket_small", "TransDate_weekday_mask", "TransDate_formatted",
        ];
        
        public static readonly string[] ExpectedExcelAwsHeaders =
        [
            "Case",
            "Store Location",
            "ServerID",
            "Requested By",
            "Requested Date",
            "Date Of Incident",
            "Session Name",
            "Channel Name",
            "Backup Time",
            "Size",
            "Status",
            "Email ( Contact info +  Receiver info)",
            "Tester (manual input from tool)",
            "Note",
            "AWS link",
            "Date download file",
        ];
    }
}