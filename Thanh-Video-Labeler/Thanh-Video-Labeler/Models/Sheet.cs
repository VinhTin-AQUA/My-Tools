using Thanh_Video_Labeler.Enums;

namespace Thanh_Video_Labeler.Models
{
    public class Sheet : Entity
    {
        public string SheetName { get; set; } = string.Empty;
        public string SheetCode { get; set; } = string.Empty;
        public ESheetStatus SheetStatus { get; set; } = ESheetStatus.Pending;
    }
}