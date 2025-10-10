using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Windows;

namespace LazyVoom.Hosting.WPF;

public static class AppHostExtensions
{
    static void Init()
    {
        Thread.CurrentThread.SetApartmentState (ApartmentState.Unknown);
        Thread.CurrentThread.SetApartmentState (ApartmentState.STA);
    }

    public static WPFHost BuildApp(this HostApplicationBuilder builder)
    {
        Init ();
        var assembly = Assembly.GetEntryAssembly ()!;

        // App 타입 찾기
        var appType = assembly.GetTypes ()
            .FirstOrDefault (t => typeof (Application).IsAssignableFrom (t)
                              && !t.IsAbstract
                              && t != typeof (Application));

        // MainWindow 타입 찾기
        var mainWindowType = assembly.GetTypes ()
            .FirstOrDefault (t => t.Name == "MainWindow"
                              && typeof (Window).IsAssignableFrom (t));

        // App이 없으면 기본 Application 사용
        if (appType == null)
        {
            appType = typeof (Application);
        }

        // 등록
        builder.Services.AddSingleton (appType);
        if (mainWindowType == null)
            throw new InvalidOperationException (
        """
        실행 가능한 Window 타입을 찾지 못했습니다.

        LazyVoom.Hosting.WPF에서는 기본적으로 'MainWindow'를 자동으로 탐색합니다.
        만약 다른 이름의 창을 사용한다면, 명시적으로 지정하세요:

            builder.BuildApp<App, MyCustomWindow>();

        또는 MainWindow 이름을 유지하세요.
        """);
        builder.Services.AddSingleton (mainWindowType);
        var host = builder.Build ();
        var app = (Application)host.Services.GetRequiredService (appType);
        
        return new WPFHost (app, host, mainWindowType);
    }

    public static WPFHost BuildApp<TApp, TMainWindow>(this HostApplicationBuilder builder)
                where TApp : Application
                where TMainWindow : Window
    {
        Init ();

        builder.Services.AddSingleton<TApp> ();
        builder.Services.AddSingleton<TMainWindow> ();

        var host = builder.Build ();
        var app = host.Services.GetRequiredService<TApp> ();

        return new WPFHost(app,host, typeof(TMainWindow));
    }
}
