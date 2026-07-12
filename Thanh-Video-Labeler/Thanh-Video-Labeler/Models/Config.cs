namespace Thanh_Video_Labeler.Models
{
    public class Config : Entity
    {
        public string ExceFileName { get; set; } = string.Empty; 
        public string SheetName { get; set; } = string.Empty;
        public string SheetCode { get; set; } = string.Empty;
    }
}