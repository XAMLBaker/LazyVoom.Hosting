using LazyVoom.Hosting.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace LazyVoom.Hosting.WPF;

public class WPFApp : VoomApp
{
    protected Type AppType { get; }
    protected Type MainWindowType { get; }
    public WPFApp(IHost host, Type appType, Type mainWindowType = null)
            : base(host)
    {
        AppType = appType;
        MainWindowType = mainWindowType;
    }

    public override void Run()
    {
        var provider = Host.Services;

        HostLifecycle.StartHostAsync (Host, provider, OnStartUpAsync);

        var app = (Application)provider.GetRequiredService (AppType);

        if (MainWindowType == null)
        {
            app.Run ();
        }
        else
        {
            var mainWindow = (Window)provider.GetRequiredService (MainWindowType);

            app.Run (mainWindow);        
        }

        HostLifecycle.StopHostAndRunExit (Host, provider, OnExitAsync);
    }
}