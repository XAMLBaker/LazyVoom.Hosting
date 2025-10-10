
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

    public static Application BuildApp(this HostApplicationBuilder builder)
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
        if (mainWindowType != null)
        {
            builder.Services.AddSingleton (mainWindowType);
        }

        var host = builder.Build ();
        var app = (Application)host.Services.GetRequiredService (appType);

        // MainWindow�� ������ �ڵ����� ����
        if (mainWindowType != null)
        {
            app.Startup += (s, e) =>
            {
                var mainWindow = (Window)host.Services.GetRequiredService (mainWindowType);
                mainWindow.Show ();
            };
            app.Exit += (s, e) => host.Dispose ();
        }

        return app;
    }

    public static Application BuildApp<TApp, TMainWindow>(this HostApplicationBuilder builder)
                where TApp : Application
                where TMainWindow : Window
    {
        Init ();

        builder.Services.AddSingleton<TApp> ();
        builder.Services.AddSingleton<TMainWindow> ();

        var host = builder.Build ();

        var app = host.Services.GetRequiredService<TApp> ();

        app.Startup += (s, e) =>
        {
            var mainWindow = host.Services.GetRequiredService<TMainWindow> ();
            mainWindow.Show ();
        };
        app.Exit += (s, e) => host.Dispose ();

        return app;
    }
}
