using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
namespace LazyVoom.Hosting.Winform;
public static class AppHostExtensions
{
    public static WinFormsHost BuildApp(this HostApplicationBuilder builder)
    {
        builder.Logging.ClearProviders (); // EventLog 제거

        // 필요한 로깅만 추가
        builder.Logging.AddConsole ();
        builder.Logging.AddDebug ();
        var assembly = Assembly.GetEntryAssembly ()!;

        // form 타입 찾기
        var formType = assembly.GetTypes ()
            .FirstOrDefault (t => t.Name == "Form1"
                              && typeof (Form).IsAssignableFrom (t));
        if (formType != null)
        {
            builder.Services.AddSingleton (formType);
        }

        var host = builder.Build ();

        return new WinFormsHost (host, formType);
    }

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