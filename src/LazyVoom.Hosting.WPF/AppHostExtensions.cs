using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;
using System.Windows;

namespace LazyVoom.Hosting.WPF;

public static class AppHostExtensions
{
    static AppHostExtensions()
    {
        // 클래스가 처음 사용될 때 자동으로 한 번만 실행
        if (Thread.CurrentThread.GetApartmentState () != ApartmentState.STA)
        {
            Thread.CurrentThread.SetApartmentState (ApartmentState.Unknown);
            Thread.CurrentThread.SetApartmentState (ApartmentState.STA);
        }
    }

    public static WPFApp BuildApp<TApp>(this HostApplicationBuilder builder)
       where TApp : Application, new()
    {
        return BuildAppCore<TApp> (builder, mainWindowType: null);
    }

    public static WPFApp BuildApp<TApp, TMainWindow>(this HostApplicationBuilder builder)
        where TApp : Application, new()
        where TMainWindow : Window
    {
        return BuildAppCore<TApp> (builder, typeof (TMainWindow));
    }

    private static WPFApp BuildAppCore<TApp>(HostApplicationBuilder builder, Type? mainWindowType)
        where TApp : Application, new()
    {
        // 1. Application 인스턴스 생성
        var appInstance = new TApp ();

        // 2. 디자인 타임이 아닐 때만 XAML 로드 시도
        if (!IsDesignMode ())
        {
            TryLoadApplicationXaml (appInstance);
        }

        // 3. DI 등록
        RegisterApplicationServices (builder.Services, appInstance, mainWindowType);

        // 4. Host 빌드 및 WPFApp 반환
        var host = builder.Build ();
        return mainWindowType != null
            ? new WPFApp (host, typeof (TApp), mainWindowType)
            : new WPFApp (host, typeof (TApp));
    }

    private static bool IsDesignMode()
    {
        return DesignerProperties.GetIsInDesignMode (new DependencyObject ());
    }

    private static void TryLoadApplicationXaml<TApp>(TApp appInstance)
        where TApp : Application
    {
        // XAML이 이미 로드되었는지 확인
        if (appInstance.Resources?.Count > 0)
        {
            return; // 생성자에서 InitializeComponent()로 이미 로드됨
        }

        try
        {
            var appType = appInstance.GetType ();
            var assemblyName = appType.Assembly.GetName ().Name;
            var xamlUri = new Uri ($"/{assemblyName};component/{appType.Name}.xaml", UriKind.Relative);

            Application.LoadComponent (appInstance, xamlUri);
        }
        catch (Exception ex)
        {
            Console.WriteLine ($"[WARN] {appInstance.GetType ().Name}.xaml 로드 실패: {ex.Message}");
        }
    }

    private static void RegisterApplicationServices<TApp>(
        IServiceCollection services,
        TApp appInstance,
        Type? mainWindowType)
        where TApp : Application
    {
        // Application 인스턴스를 여러 타입으로 등록
        services.AddSingleton<Application> (appInstance);
        services.AddSingleton (typeof (TApp), appInstance);

        // MainWindow 타입이 지정된 경우 등록
        if (mainWindowType != null)
        {
            services.AddSingleton (mainWindowType);
        }
    }
}
