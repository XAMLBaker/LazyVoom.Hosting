using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LazyVoom.Hosting.Core
{
    public class VoomApp : IVoomApp
    {
        protected IHost Host { get; }

        public Func<IServiceProvider, Task>? OnStartUpAsync { get; set; }

        // Exit 시 실행할 비동기 작업
        public Func<IServiceProvider, Task>? OnExitAsync { get; set; }

        public VoomApp(IHost host)
        {
            Host = host;
        }
        public T GetRequiredService<T>() where T : notnull
                => Host.Services.GetRequiredService<T> ();

        public virtual void Run()
        {
        }
    }
}