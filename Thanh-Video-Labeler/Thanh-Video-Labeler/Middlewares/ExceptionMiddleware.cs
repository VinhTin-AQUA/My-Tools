using System.Net;

namespace Thanh_Video_Labeler.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Đã xảy ra lỗi trong quá trình xử lý yêu cầu.");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    Message = _env.IsDevelopment()
                        ? ex.Message
                        : "Có lỗi xảy ra. Vui lòng liên hệ với quản trị viên.",
                    StackTrace = _env.IsDevelopment() ? ex.StackTrace : null,
                    InnerException = _env.IsDevelopment() ? ex.InnerException?.Message : null,
                    IsSuccess = false
                };

                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}