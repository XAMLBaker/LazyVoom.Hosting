using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LazyVoom.Hosting.Core.Extensions
{
    public class LazyVoomOptions(IServiceCollection services)
    {
        // 등록된 모듈들을 추적하기 위한 내부 리스트
        internal List<IModule> Modules { get; } = new ();

        public LazyVoomOptions AddModule<TModule>() where TModule : IModule, new()
        {
       
            var module = new TModule ();
            module.ConfigureServices (services);
            Modules.Add (module);
            return this;
        }

        public LazyVoomOptions AddModule(IModule module)
        {
            if (Modules.Any (m => m.GetType () == module.GetType ()))
            {
                return this; // 조용히 넘어가거나, 필요하면 로그를 남깁니다.
            }
            module.ConfigureServices (services);
            Modules.Add (module);
            return this;
        }
    }

    public static class HostingExtensions
    {
        [Obsolete ("이 방식은 더 이상 권장하지 않습니다. 대신 통합 진입점인 'builder.AddLazyVoom(options => options.AddModule<TModule>())'을 사용하세요. " +
          "This method is no longer recommended. Please use the unified entry point: 'builder.AddLazyVoom(options => options.AddModule<TModule>())'.")]
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

        [Obsolete ("이 방식은 더 이상 권장하지 않습니다. 대신 통합 진입점인 'builder.AddLazyVoom(options => options.AddModule(module))'을 사용하세요. " +
          "This method is no longer recommended. Please use the unified entry point: 'builder.AddLazyVoom(options => options.AddModule(module))'.")]

        public static HostApplicationBuilder AddModule(this HostApplicationBuilder builder, IModule module)
        {
            module.ConfigureServices(builder.Services);
            builder.Services.AddSingleton(module);
            return builder;
        }

        public static HostApplicationBuilder AddLazyVoom(this HostApplicationBuilder builder, Action<LazyVoomOptions> configure)
        {
            // 1. 인프라 등록
            builder.Services.AddSingleton<ILazyContext, LazyContext> ();

            // 2. 옵션을 통해 모듈 수집
            var options = new LazyVoomOptions (builder.Services);
            configure (options);

            // 3. 수집된 모듈들을 'IModule'이 아닌 고유한 래퍼나 컬렉션으로 등록
            // 그래야 나중에 호스트가 시작될 때 이놈들만 싹 뽑아서 Startup을 돌림
            foreach (var module in options.Modules)
            {
                builder.Services.AddSingleton<IModule> (module);
            }

            return builder;
        }
    }
}
