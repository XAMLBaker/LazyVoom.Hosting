namespace ModuleTest.Modules;

public class Module : IModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<UserControl1> ();
    }

    public async Task OnStartup(IServiceProvider serviceProvider)
    {

    }
}
