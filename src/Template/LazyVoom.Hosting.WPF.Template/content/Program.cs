var builder = Host.CreateApplicationBuilder ();

var app = builder.BuildApp ();  // 🔥

app.OnStartUpAsync = async provider =>
{

};

app.Run ();