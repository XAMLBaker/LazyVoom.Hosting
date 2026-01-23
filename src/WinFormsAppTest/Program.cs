using LazyVoom.Hosting.Winform;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WinFormsAppTest;

var builder = Host.CreateApplicationBuilder ();
builder.Services.AddHostedService<Worker> ();

var app = builder.BuildApp<Form1> ();  // 🔥
app.OnStartUpAsync = async provider =>
{

};
// Exit 시 정리
app.OnExitAsync = async provider =>
{
    Console.WriteLine ("앱 종료 중...");
    await Task.Delay (200);
};

app.Run ();


public class Worker : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
    }
}