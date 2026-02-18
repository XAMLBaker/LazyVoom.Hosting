using Microsoft.Extensions.DependencyInjection;

namespace LazyVoom.Hosting.Core;

public interface ILazyContext
{
    // 단순 실행 - 스코프만 보장함
    Task RunAsync(Func<IServiceProvider, Task> work);

    // 결과를 반환하는 실행 - DB 조회 등에 유용
    Task<T> RunAsync<T>(Func<IServiceProvider, Task<T>> work);
}

public class LazyContext(IServiceScopeFactory scopeFactory) : ILazyContext
{
    public async Task RunAsync(Func<IServiceProvider, Task> work)
    {
        using var scope = scopeFactory.CreateScope ();
        await work (scope.ServiceProvider);
    }

    public async Task<T> RunAsync<T>(Func<IServiceProvider, Task<T>> work)
    {
        using var scope = scopeFactory.CreateScope ();
        return await work (scope.ServiceProvider);
    }
}