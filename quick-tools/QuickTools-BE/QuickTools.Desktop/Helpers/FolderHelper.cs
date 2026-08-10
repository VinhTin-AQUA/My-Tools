using System.Diagnostics;

namespace QuickTools.Windows.Helpers
{
    public class FolderHelper
    {
        public static string GetFolder(EFolder folder)
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            const string temps = "Temps";
            
            return folder switch
            {
                EFolder.Temps => temps,
                EFolder.Iloveimg_Upscale_Temp => Path.Combine(appDirectory, temps, "Iloveimg_Upscale_Temp"),
                EFolder.Iloveimg_Upscale_Result => Path.Combine(appDirectory, temps, "Iloveimg_Upscale_Result"),
                
                _ => string.Empty
            };
        }
        
        public static string GetSystemPath(SystemFolder folder)
        {
            if (OperatingSystem.IsWindows())
                return GetWindowsPath(folder);

            if (OperatingSystem.IsLinux())
                return GetLinuxPath(folder);

            throw new PlatformNotSupportedException(
                $"Platform '{Environment.OSVersion.Platform}' is not supported.");
        }
        
        public static void OpenFolder(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Folder not found: {path}");

            string command;
            string arguments;

            if (OperatingSystem.IsWindows())
            {
                command = "explorer.exe";
                arguments = $"\"{path}\"";
            }
            else if (OperatingSystem.IsLinux())
            {
                command = "xdg-open";
                arguments = $"\"{path}\"";
            }
            else if (OperatingSystem.IsMacOS())
            {
                command = "open";
                arguments = $"\"{path}\"";
            }
            else
            {
                throw new PlatformNotSupportedException();
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                UseShellExecute = true
            });
        }
        
        #region private methods

        private static string GetWindowsPath(SystemFolder folder)
        {
            return folder switch
            {
                SystemFolder.Desktop =>
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.DesktopDirectory),

                SystemFolder.Documents =>
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.MyDocuments),

                SystemFolder.Music =>
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.MyMusic),

                SystemFolder.Videos =>
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.MyVideos),

                SystemFolder.Pictures =>
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.MyPictures),

                SystemFolder.UserProfile =>
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.UserProfile),

                SystemFolder.AppData =>
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData),

                SystemFolder.LocalAppData =>
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData),

                SystemFolder.Temp =>
                    Path.GetTempPath(),

                SystemFolder.Downloads =>
                    Path.Combine(
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.UserProfile),
                        "Downloads"),

                _ => throw new ArgumentOutOfRangeException(nameof(folder))
            };
        }

        private static string GetLinuxPath(SystemFolder folder)
        {
            var home = Environment.GetFolderPath(
                Environment.SpecialFolder.UserProfile);

            return folder switch
            {
                SystemFolder.Desktop =>
                    Path.Combine(home, "Desktop"),

                SystemFolder.Documents =>
                    Path.Combine(home, "Documents"),

                SystemFolder.Downloads =>
                    Path.Combine(home, "Downloads"),

                SystemFolder.Music =>
                    Path.Combine(home, "Music"),

                SystemFolder.Videos =>
                    Path.Combine(home, "Videos"),

                SystemFolder.Pictures =>
                    Path.Combine(home, "Pictures"),

                SystemFolder.UserProfile =>
                    home,

                SystemFolder.AppData =>
                    Path.Combine(home, ".config"),

                SystemFolder.LocalAppData =>
                    Path.Combine(home, ".local", "share"),

                SystemFolder.Temp =>
                    Path.GetTempPath(),

                _ => throw new ArgumentOutOfRangeException(nameof(folder))
            };
        }
        
        #endregion
    }

    public enum EFolder
    {
        Temps,
        Iloveimg_Upscale_Temp,
        Iloveimg_Upscale_Result,
    }
    
    public enum SystemFolder
    {
        Desktop,
        Documents,
        Downloads,
        Music,
        Videos,
        Pictures,
        UserProfile,
        AppData,
        LocalAppData,
        Temp
    }
}