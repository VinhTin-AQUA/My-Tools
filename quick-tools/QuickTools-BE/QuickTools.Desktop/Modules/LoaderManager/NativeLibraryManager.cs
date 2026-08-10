using System.Reflection;
using System.Runtime.InteropServices;

namespace QuickTools.Windows.Modules.LoaderManager
{
    #region old code
    // public class NativeLibraryManager
    // {
    //     private static readonly Dictionary<string, string> _libraryPaths = new();
    //     private static readonly Dictionary<string, IntPtr> _loadedLibraries = new();
    //     private static string _baseNativePath;
    //     private static string _currentPlatform;
    //     private static bool _initialized;
    //
    //     public static void Initialize()
    //     {
    //         if (_initialized) return;
    //
    //         var baseDir = AppDomain.CurrentDomain.BaseDirectory;
    //         _baseNativePath = Path.Combine(baseDir, "Native");
    //         _currentPlatform = GetCurrentPlatform();
    //
    //         var nativePath = Path.Combine(_baseNativePath, _currentPlatform);
    //
    //         // 1. Set environment variables
    //         AddToSearchPath(nativePath);
    //
    //         // 2. Đăng ký các thư viện
    //         RegisterLibraries();
    //
    //         // 3. QUAN TRỌNG: Đăng ký DllImportResolver
    //         NativeLibrary.SetDllImportResolver(typeof(NativeLibraryManager).Assembly, DllImportResolver);
    //
    //         _initialized = true;
    //         Console.WriteLine("NativeLibraryManager initialized successfully");
    //     }
    //
    //     private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    //     {
    //         Console.WriteLine($"Resolving: {libraryName}");
    //
    //         // Kiểm tra xem có đường dẫn đã đăng ký cho library này không
    //         if (_libraryPaths.TryGetValue(libraryName, out var libFolder))
    //         {
    //             var libFileName = GetLibraryFileName(libraryName);
    //             var fullPath = Path.Combine(libFolder, libFileName);
    //
    //             Console.WriteLine($"  Trying to load from: {fullPath}");
    //
    //             if (File.Exists(fullPath))
    //             {
    //                 if (NativeLibrary.TryLoad(fullPath, out var handle))
    //                 {
    //                     Console.WriteLine($"  Successfully loaded: {fullPath}");
    //                     _loadedLibraries[fullPath] = handle;
    //                     return handle;
    //                 }
    //
    //                 Console.WriteLine($"  Failed to load: {fullPath}");
    //             }
    //             else
    //             {
    //                 Console.WriteLine($"  File not found: {fullPath}");
    //             }
    //         }
    //         else
    //         {
    //             Console.WriteLine($"  No registered path for library: {libraryName}");
    //         }
    //
    //         // Nếu không tìm thấy, để runtime tự tìm
    //         return IntPtr.Zero;
    //     }
    //
    //     private static string GetCurrentPlatform()
    //     {
    //         if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    //             return "win-x64";
    //         if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    //             return "linux-x64";
    //         if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    //             return "osx-x64";
    //         throw new PlatformNotSupportedException("Unsupported OS");
    //     }
    //
    //     private static void RegisterLibraries()
    //     {
    //         // Đăng ký từng thư viện với tên và thư mục
    //         RegisterLibrary("webui", "webui");
    //         // RegisterLibrary("ffmpeg", "ffmpeg");
    //         // Thêm các thư viện khác nếu cần
    //     }
    //
    //     private static void RegisterLibrary(string libraryName, string folderName)
    //     {
    //         var libFolder = Path.Combine(_baseNativePath, _currentPlatform, folderName);
    //
    //         if (Directory.Exists(libFolder))
    //         {
    //             _libraryPaths[libraryName] = libFolder;
    //             Console.WriteLine($"Registered: {libraryName} -> {libFolder}");
    //
    //             // Preload dependencies (optional)
    //             PreloadDependencies(libFolder);
    //         }
    //         else
    //         {
    //             Console.WriteLine($"Warning: Folder not found for {libraryName}: {libFolder}");
    //         }
    //     }
    //
    //     private static void PreloadDependencies(string libFolder)
    //     {
    //         // Load các dependencies trong thư mục (không phải main library)
    //         var files = Directory.GetFiles(libFolder, GetLibrarySearchPattern());
    //         foreach (var file in files)
    //             if (!_loadedLibraries.ContainsKey(file))
    //                 try
    //                 {
    //                     if (NativeLibrary.TryLoad(file, out var handle))
    //                     {
    //                         _loadedLibraries[file] = handle;
    //                         Console.WriteLine($"  Preloaded: {Path.GetFileName(file)}");
    //                     }
    //                 }
    //                 catch (Exception ex)
    //                 {
    //                     Console.WriteLine($"  Failed to preload {Path.GetFileName(file)}: {ex.Message}");
    //                 }
    //     }
    //
    //     private static string GetLibraryFileName(string libraryName)
    //     {
    //         if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    //             return $"{libraryName}.dll";
    //         if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    //             return $"lib{libraryName}.so";
    //         if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    //             return $"lib{libraryName}.dylib";
    //         return libraryName;
    //     }
    //
    //     private static string GetLibrarySearchPattern()
    //     {
    //         if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    //             return "*.dll";
    //         if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    //             return "*.so*";
    //         if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    //             return "*.dylib";
    //         return "*";
    //     }
    //
    //     private static void AddToSearchPath(string nativePath)
    //     {
    //         if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    //         {
    //             var currentPath = Environment.GetEnvironmentVariable("PATH") ?? "";
    //             Environment.SetEnvironmentVariable("PATH", $"{nativePath};{currentPath}");
    //
    //             foreach (var dir in Directory.GetDirectories(nativePath))
    //                 Environment.SetEnvironmentVariable("PATH", $"{dir};{Environment.GetEnvironmentVariable("PATH")}");
    //         }
    //         else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    //         {
    //             var ldPath = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH") ?? "";
    //
    //             // Thêm nativePath và tất cả thư mục con
    //             var allPaths = nativePath;
    //             foreach (var dir in Directory.GetDirectories(nativePath)) allPaths = $"{dir}:{allPaths}";
    //
    //             Environment.SetEnvironmentVariable("LD_LIBRARY_PATH",
    //                 string.IsNullOrEmpty(ldPath) ? allPaths : $"{allPaths}:{ldPath}");
    //
    //             Console.WriteLine($"LD_LIBRARY_PATH set to: {Environment.GetEnvironmentVariable("LD_LIBRARY_PATH")}");
    //         }
    //         else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    //         {
    //             var dyldPath = Environment.GetEnvironmentVariable("DYLD_LIBRARY_PATH") ?? "";
    //
    //             var allPaths = nativePath;
    //             foreach (var dir in Directory.GetDirectories(nativePath)) allPaths = $"{dir}:{allPaths}";
    //
    //             Environment.SetEnvironmentVariable("DYLD_LIBRARY_PATH",
    //                 string.IsNullOrEmpty(dyldPath) ? allPaths : $"{allPaths}:{dyldPath}");
    //         }
    //     }
    //
    //     public static string GetLibraryPath(string libraryName)
    //     {
    //         if (_libraryPaths.TryGetValue(libraryName, out var path))
    //             return path;
    //         return null;
    //     }
    //
    //     public static bool IsLibraryLoaded(string libraryName)
    //     {
    //         return _libraryPaths.ContainsKey(libraryName);
    //     }
    // }
    
    #endregion

    public class NativeLibraryManager
    {
        private static readonly Dictionary<string, string> _libraryPaths = new();
        private static readonly Dictionary<string, IntPtr> _loadedLibraries = new();
        private static readonly HashSet<string> _loadedDependencies = new();
        private static string _baseNativePath = "";
        private static string _currentPlatform = "";
        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized) return;

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _baseNativePath = Path.Combine(baseDir, "Native");
            _currentPlatform = GetCurrentPlatform();

            var nativePath = Path.Combine(_baseNativePath, _currentPlatform);

            // 1. Set environment variables
            AddToSearchPath(nativePath);

            // 2. Đăng ký các thư viện và load dependencies
            RegisterLibraries();

            // 3. Đăng ký DllImportResolver
            NativeLibrary.SetDllImportResolver(typeof(NativeLibraryManager).Assembly, DllImportResolver);

            _initialized = true;
            Console.WriteLine("NativeLibraryManager initialized successfully");
            PrintLoadedLibraries();
        }

        private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            Console.WriteLine($"Resolving: {libraryName}");

            // Kiểm tra xem có đường dẫn đã đăng ký cho library này không
            if (_libraryPaths.TryGetValue(libraryName, out var libFolder))
            {
                var libFileName = GetLibraryFileName(libraryName);
                var fullPath = Path.Combine(libFolder, libFileName);

                if (File.Exists(fullPath))
                    if (NativeLibrary.TryLoad(fullPath, out var handle))
                    {
                        Console.WriteLine($"  ✓ Loaded: {fullPath}");
                        _loadedLibraries[fullPath] = handle;
                        return handle;
                    }

                // Nếu không tìm thấy file chính, thử tìm trong thư mục
                Console.WriteLine($"  Trying to find {libraryName} in {libFolder}");
                var files = Directory.GetFiles(libFolder, $"*{libraryName}*.so*");
                foreach (var file in files)
                    if (NativeLibrary.TryLoad(file, out var handle))
                    {
                        Console.WriteLine($"  ✓ Loaded: {file}");
                        _loadedLibraries[file] = handle;
                        return handle;
                    }
            }

            return IntPtr.Zero;
        }

        private static void RegisterLibraries()
        {
            // Đăng ký từng thư viện
            RegisterLibrary("webui-2", "webui");
            RegisterLibrary("ffmpeg", "ffmpeg");
            // Thêm các thư viện khác nếu cần
            // RegisterLibrary("opencv", "opencv");
            // RegisterLibrary("tensorflow", "tensorflow");
        }

        private static void RegisterLibrary(string libraryName, string folderName)
        {
            var libFolder = Path.Combine(_baseNativePath, _currentPlatform, folderName);

            if (Directory.Exists(libFolder))
            {
                _libraryPaths[libraryName] = libFolder;
                Console.WriteLine($"Registered: {libraryName} -> {libFolder}");

                // QUAN TRỌNG: Load tất cả dependencies trong thư mục
                LoadAllDependencies(libFolder);
            }
            else
            {
                Console.WriteLine($"Warning: Folder not found for {libraryName}: {libFolder}");
            }
        }

        private static void LoadAllDependencies(string libFolder)
        {
            Console.WriteLine($"Loading all dependencies in: {libFolder}");

            // Lấy tất cả file thư viện trong thư mục
            var files = Directory.GetFiles(libFolder, GetLibrarySearchPattern());

            // Sắp xếp: load các file nhỏ/ít phụ thuộc trước
            var sortedFiles = files.OrderBy(f => new FileInfo(f).Length).ToArray();

            foreach (var file in sortedFiles)
            {
                // Bỏ qua nếu đã load
                if (_loadedDependencies.Contains(file)) continue;

                try
                {
                    if (NativeLibrary.TryLoad(file, out var handle))
                    {
                        _loadedLibraries[file] = handle;
                        _loadedDependencies.Add(file);
                        Console.WriteLine($"  ✓ Loaded dependency: {Path.GetFileName(file)}");
                    }
                    else
                    {
                        // Có thể file này phụ thuộc vào file khác, sẽ load sau
                        Console.WriteLine($"  ⚠ Failed to load: {Path.GetFileName(file)} (will retry later)");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  ✗ Error loading {Path.GetFileName(file)}: {ex.Message}");
                }
            }
        }

        private static string GetLibraryFileName(string libraryName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return $"{libraryName}.dll";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return $"lib{libraryName}.so";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return $"lib{libraryName}.dylib";
            return libraryName;
        }

        private static string GetLibrarySearchPattern()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "*.dll";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "*.so*"; // Bắt cả *.so, *.so.1, *.so.1.2.3
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "*.dylib";
            return "*";
        }

        private static void AddToSearchPath(string nativePath)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var currentPath = Environment.GetEnvironmentVariable("PATH") ?? "";
                Environment.SetEnvironmentVariable("PATH", $"{nativePath};{currentPath}");

                foreach (var dir in Directory.GetDirectories(nativePath))
                    Environment.SetEnvironmentVariable("PATH", $"{dir};{Environment.GetEnvironmentVariable("PATH")}");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var ldPath = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH") ?? "";

                // Thêm nativePath và tất cả thư mục con
                var allPaths = nativePath;
                foreach (var dir in Directory.GetDirectories(nativePath)) allPaths = $"{dir}:{allPaths}";

                Environment.SetEnvironmentVariable("LD_LIBRARY_PATH",
                    string.IsNullOrEmpty(ldPath) ? allPaths : $"{allPaths}:{ldPath}");

                Console.WriteLine($"LD_LIBRARY_PATH: {Environment.GetEnvironmentVariable("LD_LIBRARY_PATH")}");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var dyldPath = Environment.GetEnvironmentVariable("DYLD_LIBRARY_PATH") ?? "";

                var allPaths = nativePath;
                foreach (var dir in Directory.GetDirectories(nativePath)) allPaths = $"{dir}:{allPaths}";

                Environment.SetEnvironmentVariable("DYLD_LIBRARY_PATH",
                    string.IsNullOrEmpty(dyldPath) ? allPaths : $"{allPaths}:{dyldPath}");
            }
        }

        public static string GetLibraryPath(string libraryName)
        {
            if (_libraryPaths.TryGetValue(libraryName, out var path))
                return path;
            return "";
        }

        public static bool IsLibraryLoaded(string libraryName)
        {
            return _libraryPaths.ContainsKey(libraryName);
        }

        public static void PrintLoadedLibraries()
        {
            Console.WriteLine("\n=== Loaded Libraries ===");
            foreach (var lib in _loadedLibraries) Console.WriteLine($"  {Path.GetFileName(lib.Key)}");
            Console.WriteLine($"Total: {_loadedLibraries.Count} files");
            Console.WriteLine("========================");
        }

        public static void Cleanup()
        {
            foreach (var lib in _loadedLibraries)
                if (lib.Value != IntPtr.Zero)
                    NativeLibrary.Free(lib.Value);

            _loadedLibraries.Clear();
            _loadedDependencies.Clear();
        }

        private static string GetCurrentPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "win-x64";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "linux-x64";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "osx-x64";
            throw new PlatformNotSupportedException($"Unsupported OS: {RuntimeInformation.OSDescription}");
        }
    }
}