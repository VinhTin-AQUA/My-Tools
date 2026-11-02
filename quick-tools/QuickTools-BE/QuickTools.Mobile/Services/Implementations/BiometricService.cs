using Plugin.Maui.Biometric;
using QuickTools.Mobile.Models;
using QuickTools.Mobile.Services.Interfaces;

namespace QuickTools.Mobile.Services.Implementations
{
    public class BiometricService : IBiometricService
    {
        private readonly IBiometric _biometric;

        public BiometricService()
        {
            _biometric = BiometricAuthenticationService.Default;
        }

        // Kiểm tra thiết bị có hỗ trợ sinh trắc học
        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                var status = await _biometric.GetAuthenticationStatusAsync();

                return status == BiometricHwStatus.Success;
            }
            catch
            {
                return false;
            }
        }

        // Lấy danh sách phương thức sinh trắc học đã đăng ký
        public async Task<List<string>> GetEnrolledTypesAsync()
        {
            try
            {
                var types = await _biometric.GetEnrolledBiometricTypesAsync();

                return types.Select(x => x.ToString()).ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        // Thực hiện xác thực
        public async Task<BiometricResult> AuthenticateAsync(
            string title = "Xác thực sinh trắc học",
            string description = "Vui lòng xác thực để tiếp tục")
        {
            try
            {
                var status = await _biometric.GetAuthenticationStatusAsync();

                if (status != BiometricHwStatus.Success)
                {
                    return new BiometricResult
                    {
                        Success = false,
                        Message = "Thiết bị không hỗ trợ hoặc chưa cấu hình sinh trắc học."
                    };
                }

                var request = new AuthenticationRequest
                {
                    Title = title,
                    Subtitle = description,
                    Description = description,

                    // Nội dung nút hủy xác thực
                    NegativeText = "Cancle",

                    AllowPasswordAuth = false,
                    AuthStrength = AuthenticatorStrength.Strong
                };

                var response = await _biometric.AuthenticateAsync(
                    request,
                    CancellationToken.None);

                return new BiometricResult
                {
                    Success = response.Status == BiometricResponseStatus.Success,
                    Message = response.Status == BiometricResponseStatus.Success
                        ? "Xác thực thành công"
                        : response.ErrorMsg ?? "Xác thực thất bại",
                    AuthenticationType = response.AuthenticationType.ToString()
                };
            }
            catch (Exception ex)
            {
                return new BiometricResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}