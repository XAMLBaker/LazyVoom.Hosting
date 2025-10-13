using LazyRegion.Core;
using LazyVoom.Core;
using LazyVoom.LazyRegion;

var builder = Host.CreateApplicationBuilder ();
builder.Services.UseLazyRegion ()
                .AddLazyView<ScreenA> ("a")
                .AddLazyView<ScreenB> ("b");
builder.Services.AddSingleton<MainWindowViewModel> ();

var app = builder.BuildApp<App, MainWindow> ();  // 🔥
app.OnStartUpAsync = async provider =>
{
    Voom.Instance
        .WithContainerResolver (vmType =>
        {
            return provider.GetService (vmType) ??
                   ActivatorUtilities.CreateInstance (provider, vmType);
        });
};
// Exit 시 정리
app.OnExitAsync = async provider =>
{
    Console.WriteLine ("앱 종료 중...");
    await Task.Delay (200);
};

app.Run ();
