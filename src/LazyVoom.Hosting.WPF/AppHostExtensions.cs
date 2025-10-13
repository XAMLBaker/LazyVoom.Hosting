using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace LazyVoom.Hosting.WPF;

public static class AppHostExtensions
{
    static AppHostExtensions()
    {
        // 클래스가 처음 사용될 때 자동으로 한 번만 실행
        Thread.CurrentThread.SetApartmentState (ApartmentState.Unknown);
        Thread.CurrentThread.SetApartmentState (ApartmentState.STA);
    }

    public static WPFHost BuildApp<TApp, TMainWindow>(this HostApplicationBuilder builder)
         where TApp : Application, new()
         where TMainWindow : Window
    {
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

    public static HostApplicationBuilder AddModule<TModule>(this HostApplicationBuilder builder)
        where TModule : IModule, new()
    {
        var module = new TModule ();

        // 서비스 등록
        module.ConfigureServices (builder.Services);

        // Startup 콜백 저장 (나중에 BuildApp에서 실행)
        builder.Services.AddSingleton<IModule> (module);

        return builder;
    }

    // 여러 모듈 체이닝 가능
    public static HostApplicationBuilder AddModule(this HostApplicationBuilder builder, IModule module)
    {
        module.ConfigureServices (builder.Services);
        builder.Services.AddSingleton (module);
        return builder;
    }
}
