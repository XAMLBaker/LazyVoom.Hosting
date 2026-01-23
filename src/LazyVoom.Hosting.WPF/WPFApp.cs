using LazyVoom.Hosting.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace LazyVoom.Hosting.WPF;

public class WPFApp : VoomApp
{
    protected Type AppType { get; }
    protected Type MainWindowType { get; }
    public WPFApp(IHost host, Type appType, Type mainWindowType)
            : base(host)
    {
        AppType = appType;
        MainWindowType = mainWindowType;
    }

    public override void Run()
    {
        var provider = Host.Services;
        var app = (Application)provider.GetRequiredService (AppType);

        HostLifecycle.StartHostAsync (Host, provider, OnStartUpAsync);

        var mainWindow = (Window)provider.GetRequiredService (MainWindowType);
        // ✅ UI 메시지 루프 시작 (블로킹)
        app.Run (mainWindow);        

        HostLifecycle.StopHostAndRunExit (Host, provider, OnExitAsync);
    }
}
