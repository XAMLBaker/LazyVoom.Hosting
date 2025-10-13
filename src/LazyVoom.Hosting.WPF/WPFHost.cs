using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;

namespace LazyVoom.Hosting.WPF;

public class WPFHost
{
    private readonly IHost _host;
    private readonly Type _appType;
    private readonly Type _mainWindowType;

    // Startup에서 실행할 비동기 작업
    public Func<IServiceProvider, Task>? OnStartUpAsync { get; set; }

    // Exit 시 실행할 비동기 작업
    public Func<IServiceProvider, Task>? OnExitAsync { get; set; }

    public WPFHost(IHost host, Type appType, Type mainWindowType)
    {
        _host = host;
        _appType = appType;
        _mainWindowType = mainWindowType;
    }

    public void Run()
    {
        var provider = _host.Services;
        var app = (Application)provider.GetRequiredService (_appType);
        var mainWindow = (Window)provider.GetRequiredService (_mainWindowType);

        // ✅ Run 전에 OnStartUpAsync 실행
        if (OnStartUpAsync != null)
        {
            // UI 루프 블로킹 없이 백그라운드 Task 실행
            _ = Task.Run (async () =>
            {
                try
                {
                    await OnStartUpAsync (provider);
                }
                catch (Exception ex)
                {
                    Console.WriteLine ($"[ERROR] OnStartUpAsync: {ex}");
                }
            });
        }

        // ✅ UI 메시지 루프 시작 (블로킹)
        app.Run (mainWindow);

        // ✅ 앱 종료 후 OnExitAsync 실행
        if (OnExitAsync != null)
        {
            try
            {
                OnExitAsync (provider).GetAwaiter ().GetResult ();
            }
            catch (Exception ex)
            {
                Console.WriteLine ($"[ERROR] OnExitAsync: {ex}");
            }
        }

        // ✅ Host 정리
        _host.StopAsync ().GetAwaiter ().GetResult ();
        _host.Dispose ();
    }
}
