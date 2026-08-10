using System.Runtime.InteropServices;
using System.Text.Json;
using QuickTools.Windows.Modules.WebUI.Methods;

namespace QuickTools.Windows.Modules.WebUI
{
    public static class InterfaceBinder
    {
        private static readonly Dictionary<UIntPtr, List<GCHandle>> _handles = new();
        private static webui_interface_callback_t _asyncCallback;

        /// <summary>
        /// Bind một handler với WebUI
        /// </summary>
        public static void Bind(UIntPtr window, string element, webui_interface_callback_t callback)
        {
            // Alloc GCHandle để giữ delegate
            var handle = GCHandle.Alloc(callback);
            
            if (!_handles.ContainsKey(window))
                _handles[window] = new List<GCHandle>();
            
            _handles[window].Add(handle);
            
            // Chuyển thành IntPtr và bind
            IntPtr ptr = Marshal.GetFunctionPointerForDelegate(callback);
            InterfaceMethods.webui_interface_bind(window, element, ptr);
        }
        
        /// <summary>
        /// Bind một async function - return value
        /// </summary>
        public static void BindAsyncFunction(UIntPtr window, string functionName, Func<UIntPtr, UIntPtr, IntPtr, UIntPtr, UIntPtr, Task<object>> asyncFunc)
        {
            // Tạo handler wrapper
            _asyncCallback = (w, et, el, en, bi) =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        // Gọi async function
                        var result = await asyncFunc(w, et, el, en, bi);
                        
                        // Trả về kết quả
                        string json = JsonSerializer.Serialize(result);
                        InterfaceMethods.webui_interface_set_response(w, en, json);
                    }
                    catch (Exception ex)
                    {
                        InterfaceMethods.webui_interface_set_response(w, en, $"Error: {ex.Message}");
                    }
                });
            };

            // Bind với WebUI
            IntPtr ptr = Marshal.GetFunctionPointerForDelegate(_asyncCallback);
            InterfaceMethods.webui_interface_bind(window, functionName, ptr);

            // Cấu hình async
            ConfigMethods.webui_set_config(webui_config.asynchronous_response, true);
            ConfigMethods.webui_set_event_blocking(window, false);
        }
        
        /// <summary>
        /// Bind một async function - return value
        /// </summary>
        public static void BindAsyncFunctionWithNullValue(UIntPtr window, string functionName, Func<UIntPtr, UIntPtr, IntPtr, UIntPtr, UIntPtr, Task<object?>> asyncFunc)
        {
            // Tạo handler wrapper
            _asyncCallback = (w, et, el, en, bi) =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        // Gọi async function
                        var result = await asyncFunc(w, et, el, en, bi);
                        
                        // Trả về kết quả
                        string json = JsonSerializer.Serialize(result);
                        InterfaceMethods.webui_interface_set_response(w, en, json);
                    }
                    catch (Exception ex)
                    {
                        InterfaceMethods.webui_interface_set_response(w, en, $"Error: {ex.Message}");
                    }
                });
            };

            // Bind với WebUI
            IntPtr ptr = Marshal.GetFunctionPointerForDelegate(_asyncCallback);
            InterfaceMethods.webui_interface_bind(window, functionName, ptr);

            // Cấu hình async
            ConfigMethods.webui_set_config(webui_config.asynchronous_response, true);
            ConfigMethods.webui_set_event_blocking(window, false);
        }
        
        /// <summary>
        /// Bind một async action với WebUI - no return value
        /// </summary>
        public static void BindAsyncAction(UIntPtr window, string functionName, Func<UIntPtr, UIntPtr, IntPtr, UIntPtr, UIntPtr, Task> asyncAction)
        {
            webui_interface_callback_t handler = (w, et, el, en, bi) =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await asyncAction(w, et, el, en, bi);
                        InterfaceMethods.webui_interface_set_response(w, en, "Success");
                    }
                    catch (Exception ex)
                    {
                        InterfaceMethods.webui_interface_set_response(w, en, $"Error: {ex.Message}");
                    }
                });
            };

            // Bind với WebUI
            IntPtr ptr = Marshal.GetFunctionPointerForDelegate(_asyncCallback);
            InterfaceMethods.webui_interface_bind(window, functionName, ptr);

            // Cấu hình async
            ConfigMethods.webui_set_config(webui_config.asynchronous_response, true);
            ConfigMethods.webui_set_event_blocking(window, false);
        }

        public static void Cleanup(UIntPtr window)
        {
            if (_handles.TryGetValue(window, out var handles))
            {
                foreach (var handle in handles)
                {
                    if (handle.IsAllocated)
                        handle.Free();
                }
                handles.Clear();
                _handles.Remove(window);
            }
        }

        public static void CleanupAll()
        {
            foreach (var handles in _handles.Values)
            {
                foreach (var handle in handles)
                {
                    if (handle.IsAllocated)
                        handle.Free();
                }
            }
            _handles.Clear();
        }
    }
}
