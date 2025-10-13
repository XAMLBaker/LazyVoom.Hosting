using LazyVoomHosting.Winform.App;

var builder = Host.CreateApplicationBuilder ();
var app = builder.BuildApp<Form1> ();


app.OnStartUpAsync = async provider =>
{

};

await app.RunAsync ();
