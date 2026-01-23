using LazyVoom.Hosting.Core;
using System.Windows.Controls;

namespace ModuleTest.Modules;

public class Module1 : IModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton (new UserControl());
    }

    public async Task OnStartup(IServiceProvider serviceProvider)
    {

    }
}
