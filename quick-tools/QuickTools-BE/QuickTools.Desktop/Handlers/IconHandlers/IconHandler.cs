using System.Text.Json;
using QuickTools.Core.Constants;
using QuickTools.Core.DTOs.Icons;
using QuickTools.Core.Models;
using QuickTools.Core.Responses;
using QuickTools.Services.Icons;
using QuickTools.Windows.AppSingletons;
using WebUISharp;

namespace QuickTools.Windows.Handlers.IconHandlers
{
    public static class IconHandler
    {
        public static async Task IconHandlerCheckConnection(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            try
            {
                var iconService = await IconServiceSingleton.GetInstance();
                if (iconService == null)
                {
                    var json = JsonSerializer.Serialize(new WebUIResponse<object>
                    {
                        Title = "DisConnected",
                        Action = ActionConstants.CheckConnection,
                        Data = null,
                        Description = "",
                        Success = false
                    }, options);
                    WebUI.InterfaceSetResponse(window, event_number, json);
                    return;
                }
                var jsonSuccess = JsonSerializer.Serialize(new WebUIResponse<object>
                {
                    Title = "Connected",
                    Action = ActionConstants.CheckConnection,
                    Data = null,
                    Description = "",
                    Success = true
                }, options);
                WebUI.InterfaceSetResponse(window, event_number, jsonSuccess);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ HandleGetIconsEvent error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                var json = JsonSerializer.Serialize(new WebUIResponse<object>
                {
                    Title = "Connected",
                    Action = ActionConstants.CheckConnection,
                    Data = null,
                    Description = "",
                    Success = false
                }, options);
                WebUI.InterfaceSetResponse(window, event_number, json);
            }
        }

        public static async Task<object?> GetIcons(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            try
            {
                Console.WriteLine("🔍 HandleGetIconsEvent called");

                // 1. Lấy JSON data từ WebUI
                var dataPtr = WebUI.InterfaceGetStringAt(window, event_number, UIntPtr.Zero);
                var jsonData = WebUI.PtrToString(dataPtr);

                Console.WriteLine($"📝 Raw JSON data: {jsonData ?? "(null)"}");

                // 2. Deserialize request
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Thêm dòng này
                };

                SearchIconRequest? searchIconRequest = null;

                if (!string.IsNullOrEmpty(jsonData))
                    searchIconRequest = JsonSerializer.Deserialize<SearchIconRequest>(jsonData, options);

                // Nếu không có request hoặc request null, tạo request mặc định
                if (searchIconRequest == null)
                {
                    Console.WriteLine("⚠️ Request is null, using default values");
                    searchIconRequest = new SearchIconRequest
                    {
                        Keyword = null,
                        Page = 1,
                        PageSize = 10
                    };
                }

                Console.WriteLine(
                    $"📊 Request: Page={searchIconRequest.Page}, PageSize={searchIconRequest.PageSize}, Keyword='{searchIconRequest.Keyword ?? "(null)"}'");

                // 3. Lấy dữ liệu từ service
                var iconService = await IconServiceSingleton.GetInstance();
                var icons = await iconService.SearchPaginationAsync(searchIconRequest);
                
                Console.WriteLine($"✅ Icons retrieved: {icons.Items.Count} items, Total: {icons.TotalCount}");

                var json = JsonSerializer.Serialize(icons, options);
                WebUI.InterfaceSetResponse(window, event_number, json);
                return icons;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ HandleGetIconsEvent error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                WebUI.InterfaceSetResponse(window, event_number, JsonSerializer.Serialize(new SearchIconResponse
                {
                    Items = new List<IconModel>(),
                    TotalCount = 0,
                    Page = 1,
                    PageSize = 10,
                    TotalPages = 0
                }));
                return null;
            }
        }

        public static async Task<object?> AddIcon(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var dataPtr = WebUI.InterfaceGetStringAt(window, event_number, UIntPtr.Zero);
            var jsonData = WebUI.PtrToString(dataPtr);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var addIconRequest = JsonSerializer.Deserialize<AddIconRequest>(jsonData, options);

            if (addIconRequest == null) return null;

            var iconService = await IconServiceSingleton.GetInstance();
            var icon = await iconService.CreateAsync(new IconModel
            {
                Name = addIconRequest.Name,
                Url = addIconRequest.Url,
                IconType = addIconRequest.IconType
            });

            var json = JsonSerializer.Serialize(icon, options);
            WebUI.InterfaceSetResponse(window, event_number, json);

            return icon;
        }

        public static async Task<object?> DeleteIcon(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var dataPtr = WebUI.InterfaceGetStringAt(window, event_number, UIntPtr.Zero);
            var jsonData = WebUI.PtrToString(dataPtr);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var deleteIconRequest = JsonSerializer.Deserialize<DeleteIconRequest>(jsonData, options);

            if (deleteIconRequest == null) return null;

            var iconService = await IconServiceSingleton.GetInstance();
            var r = await iconService.DeleteAsync(deleteIconRequest.Id);
            var json = JsonSerializer.Serialize(r, options);
            WebUI.InterfaceSetResponse(window, event_number, json);
            return r;
        }

        public static async Task<object?> UpdateIcon(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var dataPtr = WebUI.InterfaceGetStringAt(window, event_number, UIntPtr.Zero);
            var jsonData = WebUI.PtrToString(dataPtr);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var updateIconRequest = JsonSerializer.Deserialize<UpdateIconRequest>(jsonData, options);

            if (updateIconRequest == null) return null;

            var iconService = await IconServiceSingleton.GetInstance();
            var r = await iconService.UpdateAsync(updateIconRequest.Id, new IconModel
            {
                Url = updateIconRequest.Url,
                Name = updateIconRequest.Name
            });
            var json = JsonSerializer.Serialize(r, options);
            WebUI.InterfaceSetResponse(window, event_number, json);
            return r;
        }

        public static async Task<object?> AddMultiIcons(UIntPtr window, UIntPtr event_type, IntPtr element,
            UIntPtr event_number, UIntPtr bind_id)
        {
            var dataPtr = WebUI.InterfaceGetStringAt(window, event_number, UIntPtr.Zero);
            var jsonData = WebUI.PtrToString(dataPtr);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var addIconsRequest = JsonSerializer.Deserialize<List<AddIconRequest>>(jsonData, options);

            if (addIconsRequest == null || addIconsRequest.Count == 0) return null;

            var iconService = await IconServiceSingleton.GetInstance();
            var icons = addIconsRequest.Select(r => new IconModel
            {
                Name = r.Name,
                Url = r.Url,
                IconType = r.IconType,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToList();
            var icon = await iconService.CreateManyAsync(icons);
            var json = JsonSerializer.Serialize(icon, options);
            WebUI.InterfaceSetResponse(window, event_number, json);
            return icon;
        }
    }
}