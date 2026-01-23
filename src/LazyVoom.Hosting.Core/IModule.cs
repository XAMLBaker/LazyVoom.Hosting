using Microsoft.Extensions.DependencyInjection;

namespace LazyVoom.Hosting.Core
{
    public interface IModule
    {
        void ConfigureServices(IServiceCollection services);
        Task OnStartup(IServiceProvider serviceProvider);
    }

}
