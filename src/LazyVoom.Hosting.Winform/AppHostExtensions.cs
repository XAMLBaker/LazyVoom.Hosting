using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
namespace LazyVoom.Hosting.Winform;
public static class AppHostExtensions
{
    public static void BuildAndRun(this HostApplicationBuilder builder)
    {
        builder.Logging.ClearProviders (); // EventLog 제거

        // 필요한 로깅만 추가
        builder.Logging.AddConsole ();
        builder.Logging.AddDebug ();
        ConfigureWinFormsEnvironment ();

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
        var form = (Form)host.Services.GetRequiredService (formType);

        Application.Run (form);
    }

    public static void BuildAndRun<TForm>(this HostApplicationBuilder builder)
        where TForm : Form
    {
        builder.Logging.ClearProviders (); // EventLog 제거

        // 필요한 로깅만 추가
        builder.Logging.AddConsole ();
        builder.Logging.AddDebug ();
        ConfigureWinFormsEnvironment ();

        builder.Services.AddSingleton<TForm> ();
        var host = builder.Build ();

        var form = host.Services.GetRequiredService<TForm> ();

        Application.Run (form);
    }

    private static void ConfigureWinFormsEnvironment()
    {
        // STA 스레드 설정
        if (Thread.CurrentThread.GetApartmentState () != ApartmentState.STA)
        {
            Thread.CurrentThread.SetApartmentState (ApartmentState.Unknown);
            Thread.CurrentThread.SetApartmentState (ApartmentState.STA);
        }

        // High DPI 설정 (가장 먼저 호출되어야 함)
        SetHighDpiMode ();

        Application.OleRequired ();
        Application.EnableVisualStyles ();
        Application.SetCompatibleTextRenderingDefault (false);
    }

    private static void SetHighDpiMode()
    {
        var version = Environment.OSVersion.Version;

        // Windows 10 1703+ (Build 15063)
        if (version.Major >= 10 && version.Build >= 15063)
        {
            Application.SetHighDpiMode (HighDpiMode.PerMonitorV2);
        }
        // Windows Vista+
        else if (version.Major >= 6)
        {
            Application.SetHighDpiMode (HighDpiMode.SystemAware);
        }
    }
}