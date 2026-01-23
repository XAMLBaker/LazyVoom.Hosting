using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LazyVoom.Hosting.Core.Extensions
{
    public static class HostingExtensions
    {
        public static HostApplicationBuilder AddModule<TModule>(this HostApplicationBuilder builder)
            where TModule : IModule, new()
        {
            var module = new TModule();

            // 서비스 등록
            module.ConfigureServices(builder.Services);

            // Startup 콜백 저장 (나중에 BuildApp에서 실행)
            builder.Services.AddSingleton<IModule>(module);

            return builder;
        }

        // 여러 모듈 체이닝 가능
        public static HostApplicationBuilder AddModule(this HostApplicationBuilder builder, IModule module)
        {
            module.ConfigureServices(builder.Services);
            builder.Services.AddSingleton(module);
            return builder;
        }
    }
}
