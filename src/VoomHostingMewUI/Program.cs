using Aprillz.MewUI;
using LazyVoom.Hosting.MewUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder ();
builder.Services.AddHostedService<Worker> ();

var app = builder.BuildApp<Window> ();  // 🔥
app.OnStartUpAsync = async provider =>
{

};
// Exit 시 정리
app.OnExitAsync = async provider =>
{
    Console.WriteLine ("앱 종료 중...");
    Task.Delay (200);
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