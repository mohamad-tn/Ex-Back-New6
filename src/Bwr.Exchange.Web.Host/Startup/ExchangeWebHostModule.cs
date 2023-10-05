using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Bwr.Exchange.Configuration;

namespace Bwr.Exchange.Web.Host.Startup
{
    [DependsOn(
       typeof(ExchangeWebCoreModule))]
    public class ExchangeWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public ExchangeWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ExchangeWebHostModule).GetAssembly());
        }
    }
}
