using QuickTools.Mobile.Models;

namespace QuickTools.Mobile.Services.Interfaces
{
    public interface IBiometricService
    {
        Task<bool> IsAvailableAsync();

        Task<List<string>> GetEnrolledTypesAsync();

        Task<BiometricResult> AuthenticateAsync(
            string title = "Xác thực sinh trắc học",
            string description = "Vui lòng xác thực để tiếp tục");
    }
}