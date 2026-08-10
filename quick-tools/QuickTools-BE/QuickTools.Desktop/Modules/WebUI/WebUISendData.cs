using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using QuickTools.Windows.Modules.WebUI.Methods;

namespace QuickTools.Windows.Modules.WebUI
{
    public static class WebUISendData
    {
        // Hàm gửi một đối tượng T duy nhất
        public static void SendRaw<T>(UIntPtr window, string function, T data)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            string json = JsonSerializer.Serialize(data, options);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
    
            var ptr = Marshal.AllocHGlobal(jsonBytes.Length);
            try
            {
                Marshal.Copy(jsonBytes, 0, ptr, jsonBytes.Length);
                SendDataMethods.webui_send_raw(window, function, ptr, (UIntPtr)jsonBytes.Length);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
        
        public static void SendRaw<T>(UIntPtr window, string function, T[] data) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var totalSize = data.Length * size;
            var ptr = Marshal.AllocHGlobal(totalSize);

            try
            {
                for (var i = 0; i < data.Length; i++)
                {
                    var elementPtr = IntPtr.Add(ptr, i * size);
                    Marshal.StructureToPtr(data[i], elementPtr, false);
                }

                SendDataMethods.webui_send_raw(window, function, ptr, (UIntPtr)totalSize);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public static void SendRawClient<T>(ref webui_event_t e, string function, T[] data) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var totalSize = data.Length * size;
            var ptr = Marshal.AllocHGlobal(totalSize);

            try
            {
                for (var i = 0; i < data.Length; i++)
                {
                    var elementPtr = IntPtr.Add(ptr, i * size);
                    Marshal.StructureToPtr(data[i], elementPtr, false);
                }

                SendDataMethods.webui_send_raw_client(ref e, function, ptr, (UIntPtr)totalSize);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        // Overload for byte arrays (most common use case)
        public static void SendRaw(UIntPtr window, string function, byte[] data)
        {
            var ptr = Marshal.AllocHGlobal(data.Length);
            try
            {
                Marshal.Copy(data, 0, ptr, data.Length);
                SendDataMethods.webui_send_raw(window, function, ptr, (UIntPtr)data.Length);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public static void SendRawClient(ref webui_event_t e, string function, byte[] data)
        {
            var ptr = Marshal.AllocHGlobal(data.Length);
            try
            {
                Marshal.Copy(data, 0, ptr, data.Length);
                SendDataMethods.webui_send_raw_client(ref e, function, ptr, (UIntPtr)data.Length);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        // Overload for string (UTF-8)
        public static void SendRawString(UIntPtr window, string function, string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            SendRaw(window, function, bytes);
        }

        public static void SendRawStringClient(ref webui_event_t e, string function, string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            SendRawClient(ref e, function, bytes);
        }
    }
}