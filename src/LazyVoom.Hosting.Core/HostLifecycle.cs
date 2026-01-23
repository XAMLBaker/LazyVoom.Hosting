using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace LazyVoom.Hosting.Core
{
    public static class HostLifecycle
    {
        // 시작: UI 스레드를 블로킹하지 않고 호스트를 시작하고 종료를 대기합니다.
        public static async Task StartHostAsync(IHost host, IServiceProvider provider, Func<IServiceProvider, Task>? onStart)
        {
            try
            {
                // 앱 전용 초기화(사용자 제공)
                if (onStart != null)
                    await onStart(provider).ConfigureAwait(false);

                // 모듈 초기화
                var modules = provider.GetServices<IModule>();
                foreach (var module in modules)
                {
                    await module.OnStartup(provider).ConfigureAwait(false);
                }

                // 호스트의 HostedServices를 시작하고, 종료 신호가 올 때까지 대기
                await host.StartAsync().ConfigureAwait(false);
                await host.WaitForShutdownAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] StartHostAsync: {ex}");
            }
        }

        // 종료: OnExit 콜백 실행 후 호스트 정리
        public static void StopHostAndRunExit(IHost host, IServiceProvider provider, Func<IServiceProvider, Task>? onExit)
        {
            try
            {
                if (onExit != null)
                    onExit(provider).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] OnExitAsync: {ex}");
            }

            try
            {
                host.StopAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] StopHost: {ex}");
            }

            host.Dispose();
        }
    }
}
