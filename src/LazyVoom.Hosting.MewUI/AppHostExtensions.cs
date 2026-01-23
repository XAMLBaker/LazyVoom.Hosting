using Aprillz.MewUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace LazyVoom.Hosting.MewUI;
public static class AppHostExtensions
{
    public static MewApp BuildApp<TWin>(this HostApplicationBuilder builder)
        where TWin : Window
    {
        builder.Services.AddSingleton<TWin> ();
        var host = builder.Build ();

        return new MewApp (host, typeof(TWin));
    }
}