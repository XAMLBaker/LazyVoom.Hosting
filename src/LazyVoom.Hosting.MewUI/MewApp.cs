using Aprillz.MewUI;
using LazyVoom.Hosting.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LazyVoom.Hosting.MewUI
{
    public class MewApp : VoomApp
    {
        protected Type MainWindowType { get; }
        public MewApp(IHost host, Type mainWindowType)
            : base (host)
        {
            this.MainWindowType = mainWindowType;
        }
        public override void Run()
        {
            var provider = Host.Services;
            var win = (Window)provider.GetRequiredService (MainWindowType);

            HostLifecycle.StartHostAsync (Host, provider, OnStartUpAsync);

            Application.Run (win);

            HostLifecycle.StopHostAndRunExit (Host, provider, OnExitAsync);
        }
    }
}
