using YachtCatalog.Services;
using YachtCatalog.ViewModels;
using YachtCatalog.Pages;
namespace YachtCatalog;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddSingleton(_ => new SelectionStore(Path.Combine(FileSystem.AppDataDirectory, "selections.db3")));
        string apiAddress = DeviceInfo.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5185/" : "http://localhost:5185/";
        builder.Services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri(apiAddress), Timeout = Timeout.InfiniteTimeSpan
        });
        builder.Services.AddSingleton<CatalogApiClient>();
        builder.Services.AddSingleton<CatalogViewModel>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddTransient<SavedViewModel>();
        builder.Services.AddTransient<SavedPage>();
        builder.Services.AddTransient<EditViewModel>();
        builder.Services.AddTransient<EditPage>();
        return builder.Build();
    }
}
