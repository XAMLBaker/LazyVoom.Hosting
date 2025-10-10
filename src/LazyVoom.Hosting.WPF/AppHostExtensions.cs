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

        // App Ÿ�� ã��
        var appType = assembly.GetTypes ()
            .FirstOrDefault (t => typeof (Application).IsAssignableFrom (t)
                              && !t.IsAbstract
                              && t != typeof (Application));

        // MainWindow Ÿ�� ã��
        var mainWindowType = assembly.GetTypes ()
            .FirstOrDefault (t => t.Name == "MainWindow"
                              && typeof (Window).IsAssignableFrom (t));

        // App�� ������ �⺻ Application ���
        if (appType == null)
        {
            appType = typeof (Application);
        }

        // ���
        builder.Services.AddSingleton (appType);
        if (mainWindowType == null)
            throw new InvalidOperationException (
        """
        ���� ������ Window Ÿ���� ã�� ���߽��ϴ�.

        LazyVoom.Hosting.WPF������ �⺻������ 'MainWindow'�� �ڵ����� Ž���մϴ�.
        ���� �ٸ� �̸��� â�� ����Ѵٸ�, ��������� �����ϼ���:

            builder.BuildApp<App, MyCustomWindow>();

        �Ǵ� MainWindow �̸��� �����ϼ���.
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
