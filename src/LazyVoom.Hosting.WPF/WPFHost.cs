using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace LazyVoom.Hosting.WPF;

public class WPFHost
{
    public Func<IServiceProvider, Task>? OnStartUpAsync { get; set; }
    public Application App { get; }
    public IHost Host { get; }

    private IServiceScope? _scope;

    public WPFHost(Application app, IHost host, Type Window)
    {
        App = app;
        Host = host;

        App.Startup += async(s, e) =>
        {
            _scope = Host.Services.CreateScope ();
            if (OnStartUpAsync != null)
                await OnStartUpAsync (_scope.ServiceProvider);

            var mainWindow = (Window)_scope.ServiceProvider.GetRequiredService (Window);
            mainWindow.Show ();
        };

        App.Exit += (s, e) =>
        {
            _scope?.Dispose ();
            Host.Dispose ();
        };
    }

    public void Run() => App.Run ();
}
