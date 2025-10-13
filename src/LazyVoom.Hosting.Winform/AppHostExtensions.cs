using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace LazyVoom.Hosting.Winform;
public static class AppHostExtensions
{
    public static WinFormsHost BuildAndRun<TForm>(this HostApplicationBuilder builder)
        where TForm : Form
    {
        builder.Logging.ClearProviders (); // EventLog 제거

        // 필요한 로깅만 추가
        builder.Logging.AddConsole ();
        builder.Logging.AddDebug ();

        builder.Services.AddSingleton<TForm> ();
        var host = builder.Build ();

        return new WinFormsHost (host, typeof(TForm));
    }
}