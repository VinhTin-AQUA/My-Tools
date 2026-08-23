using System.Runtime.InteropServices;
using System.Text.Json;
using WebUISharp;

namespace QuickTools.Windows.Handlers
{
    public static class ExampleHandlers
    {
        // Xử lý các tác vụ tốn thời gian mà không block UI, như tính toán phức tạp, xử lý dữ liệu lớn.
        public static void LongTaskHandler(UIntPtr window, UIntPtr event_type, IntPtr element, UIntPtr event_number, UIntPtr bind_id)
        {
            // Lấy param
            long param = WebUI.InterfaceGetIntAt(window, event_number, UIntPtr.Zero);
            Console.WriteLine($"[LongTask] Started with param: {param}");

            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(5000);
                    long result = param * 2;

                    // Trả về kết quả
                    WebUI.InterfaceSetResponse(window, event_number, result.ToString());
                    Console.WriteLine($"[LongTask] Completed: {result}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LongTask] Error: {ex.Message}");
                    WebUI.InterfaceSetResponse(window, event_number, $"Error: {ex.Message}");
                }
            });
        }

        // Lấy dữ liệu từ Backend
        public static void GetDataHandler(UIntPtr window, UIntPtr event_type, IntPtr element, UIntPtr event_number, UIntPtr bind_id)
        {
            Console.WriteLine("[GetData] Fetching data...");

            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(1500);
                    var data = new
                    {
                        id = 123,
                        name = "Sample Data",
                        items = new[] { "Item1", "Item2", "Item3" },
                        timestamp = DateTime.Now
                    };
                    string json = JsonSerializer.Serialize(data);
                    WebUI.InterfaceSetResponse(window, event_number, json);
                    Console.WriteLine("[GetData] Data returned");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GetData] Error: {ex.Message}");
                    WebUI.InterfaceSetResponse(window, event_number, $"Error: {ex.Message}");
                }
            });
        }

        // Gửi dữ liệu xuống Backend
        public static void SendDataHandler(UIntPtr window, UIntPtr event_type, IntPtr element, UIntPtr event_number, UIntPtr bind_id)
        {
            // Lấy string data từ event
            IntPtr dataPtr = WebUI.InterfaceGetStringAt(window, event_number, UIntPtr.Zero);
            string jsonData = WebUI.PtrToString(dataPtr);
            Console.WriteLine($"[SendData] Received: {jsonData}");

            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(2000);
                    
                    // Parse và validate data
                    if (!string.IsNullOrEmpty(jsonData))
                    {
                        using var doc = JsonDocument.Parse(jsonData);
                        var root = doc.RootElement;
                        
                        if (root.TryGetProperty("name", out var nameElement) &&
                            root.TryGetProperty("email", out var emailElement))
                        {
                            string name = nameElement.GetString() ?? "";
                            string email = emailElement.GetString() ?? "";
                            Console.WriteLine($"[SendData] Parsed: Name={name}, Email={email}");
                        }
                    }
                    
                    WebUI.InterfaceSetResponse(window, event_number, "Data saved successfully!");
                    Console.WriteLine("[SendData] Data saved");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SendData] Error: {ex.Message}");
                    WebUI.InterfaceSetResponse(window, event_number, $"Error: {ex.Message}");
                }
            });
        }

        // Xử lý request với ID riêng, thường dùng cho async operations cần theo dõi trạng thái.
        public static void RequestDataHandler(UIntPtr window, UIntPtr event_type, IntPtr element, UIntPtr event_number, UIntPtr bind_id)
        {
            IntPtr requestIdPtr = WebUI.InterfaceGetStringAt(window, event_number, UIntPtr.Zero);
            string requestId = WebUI.PtrToString(requestIdPtr);
            Console.WriteLine($"[RequestData] Request ID: {requestId}");

            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(3000);
                    var response = new
                    {
                        requestId = requestId,
                        status = "success",
                        data = "Response data from backend",
                        timestamp = DateTime.Now
                    };
                    string json = JsonSerializer.Serialize(response);
                    WebUI.InterfaceSetResponse(window, event_number, json);
                    Console.WriteLine($"[RequestData] Response sent for: {requestId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RequestData] Error: {ex.Message}");
                    var errorResponse = new
                    {
                        requestId = requestId,
                        status = "error",
                        error = ex.Message
                    };
                    string json = JsonSerializer.Serialize(errorResponse);
                    WebUI.InterfaceSetResponse(window, event_number, json);
                }
            });
        }
        
        // Async Function thuần túy
        public static async Task<object> MyAsyncFunction(UIntPtr window, UIntPtr event_type, IntPtr element, UIntPtr event_number, UIntPtr bind_id)
        {
            // Giả lập xử lý async với Task.Delay
            Console.WriteLine("[MyAsyncFunction] Step 1: Fetching data...");
            await Task.Delay(1000);
            
            Console.WriteLine("[MyAsyncFunction] Step 2: Processing data...");
            await Task.Delay(1000);
            
            Console.WriteLine("[MyAsyncFunction] Step 3: Saving to database...");
            await Task.Delay(1000);
            
            // Trả về kết quả
            return new
            {
                id = 123,
                name = "Sample Data",
                status = "completed",
                processedAt = DateTime.Now,
                items = new[] { "Item1", "Item2", "Item3" }
            };
        }
        
        // ==================== Async Function trả về object ====================
        public static async Task<object> UploadFileAsync(UIntPtr window, UIntPtr event_type, IntPtr element, UIntPtr event_number, UIntPtr bind_id)
        {
            // Lấy dữ liệu từ JavaScript
            IntPtr dataPtr = WebUI.InterfaceGetStringAt(window, event_number, UIntPtr.Zero);
            string jsonData = WebUI.PtrToString(dataPtr);
            
            Console.WriteLine($"[UploadFile] Received data length: {jsonData?.Length ?? 0}");

            // Parse JSON
            using var doc = JsonDocument.Parse(jsonData ?? "");
            var root = doc.RootElement;
            
            string fileName = root.GetProperty("fileName").GetString() ?? "";
            string fileContent = root.GetProperty("fileContent").GetString() ?? "";
            
            Console.WriteLine($"[UploadFile] File name: {fileName}");

            // Tạo thư mục uploads
            string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            // Decode Base64 thành byte array
            byte[] fileBytes = Convert.FromBase64String(fileContent);

            // Lưu file
            string safeFileName = $"{DateTime.Now:yyyyMMdd_HHmmss}_{Path.GetFileName(fileName)}";
            string filePath = Path.Combine(uploadDir, safeFileName);
            
            await File.WriteAllBytesAsync(filePath, fileBytes);
            
            Console.WriteLine($"[UploadFile] File saved: {filePath} ({fileBytes.Length} bytes)");

            // Trả về kết quả
            return new
            {
                status = "success",
                fileName = safeFileName,
                filePath = filePath,
                size = fileBytes.Length,
                message = "File uploaded successfully!"
            };
        }
    }
}