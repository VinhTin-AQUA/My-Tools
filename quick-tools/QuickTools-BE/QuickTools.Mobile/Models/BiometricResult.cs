namespace QuickTools.Mobile.Models
{
    public class BiometricResult
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public string? AuthenticationType { get; set; }
    }
}