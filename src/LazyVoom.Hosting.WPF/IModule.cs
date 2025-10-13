using Microsoft.Extensions.DependencyInjection;

namespace LazyVoom.Hosting.WPF
{
    public interface IModule
    {
        void ConfigureServices(IServiceCollection services);
        Task OnStartup(IServiceProvider serviceProvider);
    }

}
