using LazyVoom.Hosting.Core.Extensions;
using ModuleTest;
using ModuleTest.Modules;

var builder = Host.CreateApplicationBuilder ();


//var app = builder.AddModule<Module>()
//                 .AddModule<Module1>()
//                 .BuildApp<App, MainWindow> ();  // 🔥

var app = builder.AddLazyVoom (options =>
{
    options.AddModule<Module> ()
           .AddModule<Module1> ();
})
.BuildApp<App, MainWindow> ();  // 🔥

app.OnStartUpAsync = async provider =>
{

};

app.Run ();
