using ModuleTest;
using ModuleTest.Modules;

var builder = Host.CreateApplicationBuilder ();

var app = builder.AddModule<Module>()
                 .AddModule<Module1>()
                 .BuildApp<App, MainWindow> ();  // 🔥

app.OnStartUpAsync = async provider =>
{

};

app.Run ();
