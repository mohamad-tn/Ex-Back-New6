using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Bwr.Exchange.Authorization;

namespace Bwr.Exchange
{
    [DependsOn(
        typeof(ExchangeCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class ExchangeApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<ExchangeAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(ExchangeApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
