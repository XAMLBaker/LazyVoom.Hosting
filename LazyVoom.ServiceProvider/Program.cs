using LazyVoom.Core;
using LazyVoom.ServiceProvider;

var builder = Host.CreateApplicationBuilder ();

builder.Services.AddSingleton<MainWindowViewModel> ();
var app = builder.BuildApp ();  // 🔥

app.OnStartUpAsync = async provider =>
{
    Voom.Instance
        .WithContainerResolver (vmType =>
    {
        return provider.GetService (vmType) ?? 
               ActivatorUtilities.CreateInstance (provider, vmType);
    });
};

app.Run ();
