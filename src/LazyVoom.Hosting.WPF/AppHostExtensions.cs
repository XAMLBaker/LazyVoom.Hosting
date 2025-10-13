using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;
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

    /// <summary>
    /// AOT 이슈 발생우려로 X
    /// </summary>
    /// <typeparam name="TApp"></typeparam>
    /// <typeparam name="TMainWindow"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    //public static WPFHost BuildApp(this HostApplicationBuilder builder)
    //{
    //    Init ();
    //    var assembly = Assembly.GetEntryAssembly ()!;

    //    // App 타입 찾기
    //    var appType = assembly.GetTypes ()
    //        .FirstOrDefault (t => t.Name == "App"
    //                          && typeof (Application).IsAssignableFrom (t));

    //    // MainWindow 타입 찾기
    //    var mainWindowType = assembly.GetTypes ()
    //        .FirstOrDefault (t => t.Name == "MainWindow"
    //                          && typeof (Window).IsAssignableFrom (t));

    //    // App이 없으면 기본 Application 사용
    //    if (appType == null)
    //    {
    //        appType = typeof (Application);
    //    }

    //    // 등록
    //    builder.Services.AddSingleton (appType);
    //    if (mainWindowType == null)
    //        throw new InvalidOperationException (
    //    """
    //    실행 가능한 Window 타입을 찾지 못했습니다.

    //    LazyVoom.Hosting.WPF에서는 기본적으로 'MainWindow'를 자동으로 탐색합니다.
    //    만약 다른 이름의 창을 사용한다면, 명시적으로 지정하세요:

    //        builder.BuildApp<App, MyCustomWindow>();

    //    또는 MainWindow 이름을 유지하세요.
    //    """);
    //    builder.Services.AddSingleton (mainWindowType);
    //    var host = builder.Build ();
    //    var app = (Application)host.Services.GetRequiredService (appType);

    //    return new WPFHost (app, host, mainWindowType);
    //}

    public static WPFHost BuildApp<TApp, TMainWindow>(this HostApplicationBuilder builder)
         where TApp : Application, new()
         where TMainWindow : Window
    {
        Init ();

        // 1. Application 인스턴스 생성 (AOT-safe)
        var appInstance = new TApp ();

        // 2. 디자인 타임인지 확인
        bool isDesignMode = DesignerProperties.GetIsInDesignMode (new DependencyObject ());

        if (!isDesignMode)
        {
            // 3. Application.Current 지정
            var field = typeof (Application).GetField ("s_appInstance", BindingFlags.Static | BindingFlags.NonPublic);
            if (field != null)
                field.SetValue (null, appInstance);
            else
                typeof (Application)
                    .GetProperty ("Current", BindingFlags.Static | BindingFlags.NonPublic)?
                    .SetValue (null, appInstance);

            // 4. App.xaml 로드
            var appAssemblyName = appInstance.GetType ().Assembly.GetName ().Name;
            var appUri = new Uri ($"/{appAssemblyName};component/app.xaml", UriKind.Relative);
            try
            {
                Application.LoadComponent (appInstance, appUri);
            }
            catch (Exception ex)
            {
                Console.WriteLine ($"[WARN] App.xaml 로드 실패: {ex.Message}");
            }
        }

        // 5. DI 등록 — 한 인스턴스로 여러 타입 참조 가능
        builder.Services.AddSingleton (appInstance);              // Application 타입
        builder.Services.AddSingleton (typeof (TApp), appInstance); // TApp(App) 타입
        builder.Services.AddSingleton<TMainWindow> ();

        var host = builder.Build ();
        return new WPFHost (host, typeof (TApp), typeof (TMainWindow));
    }
}
