using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace LazyVoom.Hosting.Winform;
public static class AppHostExtensions
{
    static AppHostExtensions()
    {
        // 클래스가 처음 사용될 때 자동으로 한 번만 실행
        if (Thread.CurrentThread.GetApartmentState () != ApartmentState.STA)
        {
            Thread.CurrentThread.SetApartmentState (ApartmentState.Unknown);
            Thread.CurrentThread.SetApartmentState (ApartmentState.STA);
        }
    }

    public static WinFormsApp BuildApp<TForm>(this HostApplicationBuilder builder)
        where TForm : Form
    {
        builder.Services.AddSingleton<TForm> ();
        var host = builder.Build ();

        return new WinFormsApp (host, typeof(TForm));
    }
}