using Aprillz.MewUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace LazyVoom.Hosting.MewUI;
public static class AppHostExtensions
{
    public static MewApp BuildApp<TWin>(this HostApplicationBuilder builder)
        where TWin : Window
    {
        builder.Logging.ClearProviders (); // EventLog 제거

        // 필요한 로깅만 추가
        builder.Logging.AddConsole ();
        builder.Logging.AddDebug ();

        builder.Services.AddSingleton<TWin> ();
        var host = builder.Build ();

        return new MewApp (host, typeof(TWin));
    }
}