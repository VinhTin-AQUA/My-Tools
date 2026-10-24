namespace QuickTools.Core.DTOs.GoogleSheetPasswordManagerDTOs
{
    public class DeleteGoogleSheetPasswordManagerRequest
    {
        public int Id { get; set; }
    }

    public class SetKeyGoogleSheetPasswordManagerRequest
    {
        public string Key { get; set; } = string.Empty;
    }
    
    public class VerifyKeyGoogleSheetPasswordManagerRequest
    {
        public string Key { get; set; } = string.Empty;
    }
    
    public class VerifyKeyGoogleSheetPasswordManagerResponse
    {
        public bool IsValid { get; set; }
    }
    
    public class SetUrlGoogleSheetPasswordManagerRequest
    {
        public string Url { get; set; } = string.Empty;
    }
    
    public class SetUrlGoogleSheetPasswordManagerResponse
    {
        public string Url { get; set; } = string.Empty;
    }
    
    public class GetUrlGoogleSheetPasswordManagerResponse
    {
        public string Url { get; set; } = string.Empty;
    }
}