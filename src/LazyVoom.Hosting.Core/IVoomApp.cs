namespace LazyVoom.Hosting.Core
{
    public interface IVoomApp
    {
        Func<IServiceProvider, Task>? OnStartUpAsync { get; set; }

        // Exit 시 실행할 비동기 작업
        Func<IServiceProvider, Task>? OnExitAsync { get; set; }
        void Run();
    }
}
