using Microsoft.Extensions.Logging;
using QuickTools.Mobile.Components.Themes;
using QuickTools.Mobile.Services.Implementations;
using QuickTools.Mobile.Services.Interfaces;
using Plugin.LocalNotification;

namespace QuickTools.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        builder.UseLocalNotification();

        builder.Services.AddMauiBlazorWebView();
        
        builder.Services.AddScoped<ThemeService>();
        builder.Services.AddScoped<IFileStorageService, FileStorageService>();
        builder.Services.AddScoped<IExternalStoreService, ExternalStoreService>();
        builder.Services.AddSingleton<NotificationService>();
        
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}